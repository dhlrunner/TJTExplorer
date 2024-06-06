using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace TJTExplorer.Class
{
    internal class TJTar : IDisposable
    {
        public List<TJTarFile> Files { get; private set; }
        public string Version { get; private set; }
        public DateTime ContentStartDate { get; private set; }
        public DateTime ContentEndDate { get; private set; }

        //Const value
        private const uint fileHeaderSize = 0x84;
        private const uint tjtHeaderSize = 0x2A;     
        private const uint writerBufferSize = 4096;
        private const uint fileNameSize = 0x80;

        private BinaryReader tjtReader = null;
        private TJCRC16Calculator crc16 = null;
        private uint lastFileIndex = 0;
        

        public TJTar() 
        {
            this.Files = new List<TJTarFile>();           
            this.ContentStartDate = DateTime.Now;
            this.ContentEndDate = DateTime.Now;
            this.Version = "V1.00";
        }

        private bool CheckTJTCRC(ushort originCRC, uint bodyStartOffset)
        {
            if (tjtReader == null) throw new NullReferenceException("tjtReader is null");

            this.crc16 = new TJCRC16Calculator();
            tjtReader.BaseStream.Seek(2, SeekOrigin.Begin);
            crc16.Append(tjtReader.ReadBytes((int)tjtHeaderSize));
            short crc1 = crc16.CRC;
            crc16.Append(tjtReader.ReadBytes((int)(bodyStartOffset - tjtHeaderSize - 2)));
            short crc2 = crc16.CRC;
            short finalCRC = (short)(crc1 + crc2);

            Debug.WriteLine($"crc_header={crc1}, crc_file_header={crc2}, final={finalCRC}");
            Debug.WriteLine($"calculated_crc={finalCRC}, expected_crc={(short)originCRC}");

            return finalCRC == (short)originCRC;
        }

        

        private bool CheckSizeOver(ulong newSize)
        {                     
            return (GetTotalFileSize() + newSize) > uint.MaxValue;
        }

        private void RealignFiles()
        {
            //재정렬
            this.Files = this.Files.OrderBy(f => f.Name).ToList();
            for (int i = 0; i < this.Files.Count; i++)
            {
                this.Files[i].Index = (uint)i;
            }
            lastFileIndex = (uint)this.Files.Count;
        }
          
        public void LoadFromFile(string filename)
        {

            tjtReader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            ushort TJTarCRC16 = tjtReader.ReadUInt16();
            this.Version = Encoding.ASCII.GetString(tjtReader.ReadBytes(0x0E)).Split('\0')[0];
            uint unk = tjtReader.ReadUInt32();
            uint FileCount = tjtReader.ReadUInt32();
            uint bodySize = tjtReader.ReadUInt32();
            string startD = Encoding.ASCII.GetString(tjtReader.ReadBytes(8)).Split('\0')[0];
            string endD = Encoding.ASCII.GetString(tjtReader.ReadBytes(8)).Split('\0')[0];

            this.ContentStartDate = DateTime.ParseExact(startD, "yyyyMMdd", CultureInfo.InvariantCulture);
            this.ContentEndDate = DateTime.ParseExact(endD, "yyyyMMdd", CultureInfo.InvariantCulture);
            //CRC16 + TJT헤더크기 + (파일헤더크기 * 파일개수) = 데이터 시작 오프셋
            uint dataStartOffset = (FileCount * fileHeaderSize) + tjtHeaderSize + 2;

            if (!CheckTJTCRC(TJTarCRC16, dataStartOffset))
            {
                throw new TJTarCRCException("TJTar CRC does not match");
            }

            tjtReader.BaseStream.Seek(tjtHeaderSize + 2, SeekOrigin.Begin);

            uint lastFileSize = 0;
            for (uint i = 0; i < FileCount; i++)
            {
                string fname = Encoding.ASCII.GetString(tjtReader.ReadBytes((int)fileNameSize)).Split('\0')[0];
                uint fsize = tjtReader.ReadUInt32();
                uint fIdx = i;
                uint fOffset = dataStartOffset + lastFileSize;

                lastFileSize = lastFileSize + fsize;

                var fileInfo = new TJTarFile();
                fileInfo.Name = fname;
                fileInfo.Size = fsize;
                fileInfo.Index = fIdx;
                fileInfo.StartOffset = fOffset;
                fileInfo.IsNew = false;

                this.lastFileIndex = fIdx + 1;

                Files.Add(fileInfo);
            }
            
            
        }
        public ulong GetTotalFileSize()
        {
            ulong tSize = 0;
            foreach (TJTarFile file in this.Files)
            {
                if (file.IsNew)
                {
                    tSize = tSize + (ulong)new FileInfo(file.RealFilePath).Length;
                }
                else
                {
                    tSize = tSize + file.Size;
                }
            }

            return tSize;
        }
        public TJTarFile GetFileHeaderByIndex(uint fileIndex)
        {
            var res = this.Files.Where(n => n.Index == fileIndex);
            return res.First();
        }

        public async Task ExtractFileAsync(TJTarFile file, string outFileName)
        {

            using (BinaryWriter writer = new BinaryWriter(new FileStream(outFileName, FileMode.Create)))
            {
                await Task.Run(() => {
                    this.tjtReader.BaseStream.Seek(file.StartOffset, SeekOrigin.Begin);

                    uint amari = file.Size % writerBufferSize;
                    uint kake = file.Size / writerBufferSize;

                    //Console.WriteLine($"amari {amari} kake {kake}");

                    for (int i = 0; i < kake; i++)
                    {
                        byte[] _buffer = this.tjtReader.ReadBytes((int)writerBufferSize);
                        writer.Write(_buffer);
                    }

                    byte[] buffer = this.tjtReader.ReadBytes((int)amari);
                    writer.Write(buffer);
                    writer.Flush();
                });

            }

           
        }

        public byte[] GetFileBytes(TJTarFile file)
        {
            if (!file.IsNew)
            {
                this.tjtReader.BaseStream.Seek(file.StartOffset, SeekOrigin.Begin);
                return this.tjtReader.ReadBytes((int)file.Size);
            }
            else
            {
                throw new Exception($"This file does not included in the existing TJT.");
            }
            
        }

        public void SetContentDate(DateTime startDate, DateTime endDate)
        {
            this.ContentStartDate = startDate;
            this.ContentEndDate = endDate;
        }



        public async Task AddFilesAsync((string fileName, string fullFilePath)[]files , IProgress<int> progress = null)
        {
            await Task.Run(() =>
            {
                //Check size over
                ulong sz = 0;
                foreach (var f in files)
                {
                    if (File.Exists(f.fullFilePath))
                    {
                        ulong s = (ulong)new FileInfo(f.fullFilePath).Length;
                        sz = sz + s;
                    }
                    else
                    {
                        throw new FileNotFoundException($"Cannot find {f.fullFilePath}");
                    }
                }

                //Throw error if over then 4GB
                if (CheckSizeOver(sz))
                {
                    throw new TJTarFileSizeOverException($"Total file size is over 4GB. TJT cannot be made larger than 4GB.");

                }

                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = files[i].fileName.Replace("\\", "/");
                    if (fileName.Length > 0x80) throw new Exception($"Filename {files[i].fileName} is too long.");
                    string filePath = files[i].fullFilePath;
                    uint size = (uint)new FileInfo(filePath).Length;
                    if (File.Exists(filePath))
                    {
                        TJTarFile file = new TJTarFile();
                        file.Name = fileName;
                        file.RealFilePath = filePath;
                        file.IsNew = true;
                        file.Size = size;
                        //file.Index = lastFileIndex;
                        this.Files.Add(file);
                        //lastFileIndex++;
                        Console.WriteLine(filePath);
                    }



                    if (progress != null) progress.Report(i);
                }

                RealignFiles();
            });
            

        }

        public async Task BuildAsync(string outTJTFileName, IProgress<int> progress = null)
        {
            await Task.Run(() =>
            {
                
                using(BinaryWriter outTJT = new BinaryWriter(new FileStream(outTJTFileName, FileMode.Create)))
                {

                    List<byte> tjtHeader = new List<byte>();
                    List<byte> tjtFileHeader = new List<byte>();
                    short crc = 0;

                    //Create TJT Header
                    //Version
                    tjtHeader.AddRange(Encoding.ASCII.GetBytes(this.Version.PadRight(0x0E,'\0')));
                    tjtHeader.AddRange(BitConverter.GetBytes((uint)0)); //unk
                    tjtHeader.AddRange(BitConverter.GetBytes((uint)this.Files.Count));
                    tjtHeader.AddRange(BitConverter.GetBytes((uint)GetTotalFileSize()));
                    tjtHeader.AddRange(Encoding.ASCII.GetBytes(this.ContentStartDate.ToString("yyyyMMdd")));
                    tjtHeader.AddRange(Encoding.ASCII.GetBytes(this.ContentEndDate.ToString("yyyyMMdd")));


                    //Create File Header
                    foreach (var file in this.Files)
                    {
                        tjtFileHeader.AddRange(Encoding.ASCII.GetBytes(file.Name.PadRight(0x80, '\0')));
                        if (file.IsNew)
                        {
                            tjtFileHeader.AddRange(BitConverter.GetBytes((uint)new FileInfo(file.RealFilePath).Length));
                        }
                        else
                        {
                            tjtFileHeader.AddRange(BitConverter.GetBytes(file.Size));
                        }
                    }

                    //Init CRC Calculator
                    this.crc16 = new TJCRC16Calculator();

                    //Calculate CRC
                    this.crc16.Append(tjtHeader.ToArray());
                    short crc1 = this.crc16.CRC;
                    this.crc16.Append(tjtFileHeader.ToArray());
                    short crc2 = this.crc16.CRC;
                    crc = (short)(crc1 + crc2);

                    //Write out data
                    outTJT.Write(crc);
                    outTJT.Write(tjtHeader.ToArray());
                    outTJT.Write(tjtFileHeader.ToArray());

                    //Process 리포트용
                    int processedFileCount = 0;

                    foreach (var file in this.Files)
                    {
                        if(file.IsNew) 
                        {
                            //Todo: 메모리 절약을 위해 BinaryReader로 버퍼 크기씩 읽어들인다음 쓰는 로직으로 바꾸기
                            byte[] t = File.ReadAllBytes(file.RealFilePath);
                            outTJT.Write(t);

                        }
                        else
                        {
                            outTJT.Write(GetFileBytes(file));
                        }
                        processedFileCount++;
                        if (progress != null) progress.Report(processedFileCount);                  
                    }
                     
                }

            });
        }

        public virtual void Dispose()
        {
            if(this.tjtReader != null)
                this.tjtReader.Dispose();
        }
    }

    internal class TJTarFile
    {
        //기존 파일 읽어들였을때
        public string Name;      
        public uint Size;
        public uint Index;
        public uint StartOffset;

        //새롭게 추가된 파일일때
        public string RealFilePath;

        //기존파일이면 True
        public bool IsNew;
    }

    //Custom Exception
    public class TJTarFileSizeOverException : Exception
    {
        public TJTarFileSizeOverException()
        {
        }

        public TJTarFileSizeOverException(string message)
            : base(message)
        {
        }

        public TJTarFileSizeOverException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class TJTarCRCException : Exception
    {
        public TJTarCRCException()
        {
        }

        public TJTarCRCException(string message)
            : base(message)
        {
        }

        public TJTarCRCException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
