using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace TJTExplorer.Class
{
    internal class TJTarVersionStr
    {
        public static readonly string VER100 = "V1.00";
        public static readonly string VER101 = "V1.01";
    }
    internal class TJTar : IDisposable
    {
        public List<TJTarFile> Files { get; private set; }
        public string Version { get; private set; }
        public DateTime ContentStartDate { get; private set; }
        public DateTime ContentEndDate { get; private set; }

        //Const value
        private const uint fileHeaderSize = 0x84;
        private const uint fileHeaderSize_V101 = 0x108;
        private const uint tjtHeaderSize = 0x2A;
        private const uint tjtHeaderSize_V101 = 0x40;        
        private const uint fileNameSize = 0x80;
        private const uint fileNameSize_V101 = 0x100;
        private const uint writerBufferSize = 4096;

        private BinaryReader tjtReader = null;
        private TJCRC16Calculator crc16 = null;
        private uint lastFileIndex = 0;


        public TJTar()
        {
            this.Files = new List<TJTarFile>();
            this.ContentStartDate = DateTime.Now;
            this.ContentEndDate = DateTime.Now;
            this.Version = TJTarVersionStr.VER100;
        }

        private bool CheckTJTCRC(ushort originCRC, uint bodyStartOffset)
        {
            if (tjtReader == null) throw new NullReferenceException("tjtReader is null");
            this.crc16 = new TJCRC16Calculator();

            if (this.Version == TJTarVersionStr.VER100)
            {              
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
            else if (this.Version == TJTarVersionStr.VER101)
            {
                //tjtReader.BaseStream.Seek(2, SeekOrigin.Begin);
                //crc16.Append(tjtReader.ReadBytes((int)(tjtReader.BaseStream.Length - 2)));
                //short crc1 = crc16.CRC;
                //crc16.Append(tjtReader.ReadBytes((int)(bodyStartOffset - tjtHeaderSize_V101 - 2)));
                //short crc2 = crc16.CRC;
                //short finalCRC = (short)(crc1 + crc2);

                //Debug.WriteLine($"V1.01 crc_header={crc1}, crc_file_header={crc2}, final={finalCRC}");
                //Debug.WriteLine($"V1.01 calculated_crc={finalCRC}, expected_crc={(short)originCRC}");

                //return finalCRC == (short)originCRC;

                //아직 계산방법 모름
                return true;
            }
            else
            {
                throw new TJTarCRCException("Not supported TJT Version");
            }
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

            if (this.Version == TJTarVersionStr.VER100)
            {
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

                ulong lastFileSize = 0;
                for (uint i = 0; i < FileCount; i++)
                {
                    string fname = Encoding.ASCII.GetString(tjtReader.ReadBytes((int)fileNameSize)).Split('\0')[0];
                    uint fsize = tjtReader.ReadUInt32();
                    uint fIdx = i;
                    ulong fOffset = dataStartOffset + lastFileSize;

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
            else if(this.Version == TJTarVersionStr.VER101)
            {
                //일단 모르는 부분은 스킵
                tjtReader.BaseStream.Seek(0x32, 0);

                uint FileCount = tjtReader.ReadUInt32();
                uint unknown  = tjtReader.ReadUInt32();
                uint bodySize = tjtReader.ReadUInt32();
                

                //CRC16 + TJT헤더크기 + (파일헤더크기 * 파일개수) = 데이터 시작 오프셋
                uint dataStartOffset = (FileCount * fileHeaderSize_V101) + tjtHeaderSize_V101 + 2;

                if (!CheckTJTCRC(TJTarCRC16, dataStartOffset))
                {
                    throw new TJTarCRCException("TJTar V1.01 CRC does not match");
                }

                tjtReader.BaseStream.Seek(tjtHeaderSize_V101 + 2, SeekOrigin.Begin);

                ulong lastFileSize = 0;
                for (uint i = 0; i < FileCount; i++)
                {
                    string fname = Encoding.ASCII.GetString(tjtReader.ReadBytes((int)fileNameSize_V101)).Split('\0')[0];
                    uint fsize = tjtReader.ReadUInt32();
                    uint unk = tjtReader.ReadUInt32();
                    uint fIdx = i;
                    ulong fOffset = dataStartOffset + lastFileSize;

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
            else
            {
                throw new TJTarNotSupportedVersionException($"tjt version {this.Version} is not supported yet.");
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
                    this.tjtReader.BaseStream.Seek((long)file.StartOffset, SeekOrigin.Begin);

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

        public void ExtractFile(TJTarFile file, string outFileName)
        {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(outFileName, FileMode.Create)))
            {
                this.tjtReader.BaseStream.Seek((long)file.StartOffset, SeekOrigin.Begin);

                uint amari = file.Size % writerBufferSize;
                uint kake = file.Size / writerBufferSize;

                for (int i = 0; i < kake; i++)
                {
                    byte[] _buffer = this.tjtReader.ReadBytes((int)writerBufferSize);
                    writer.Write(_buffer);
                }

                byte[] buffer = this.tjtReader.ReadBytes((int)amari);
                writer.Write(buffer);
                writer.Flush();

            }
        }

        public byte[] GetFileBytes(TJTarFile file)
        {
            if (!file.IsNew)
            {
                this.tjtReader.BaseStream.Seek((long)file.StartOffset, SeekOrigin.Begin);
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


        /// <summary>
        /// (비동기) 파일들 추가
        /// </summary>
        /// <param name="files">파일명, 전체파일 경로 튜플 배열</param>
        /// <param name="progress">진행률 보고용</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="TJTarFileSizeOverException"></exception>
        /// <exception cref="TJTarFileNameIsTooLongException"></exception>
        public async Task AddFilesAsync((string fileName, string fullFilePath)[] files, IProgress<int> progress = null)
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
                    if (fileName.Length > 0x80) throw new TJTarFileNameIsTooLongException($"Filename {files[i].fileName} is too long.");
                    string filePath = files[i].fullFilePath;
                    ulong size = (ulong)new FileInfo(filePath).Length;

                    if (size > uint.MaxValue)
                    {
                        throw new TJTarFileSizeOverException("단일 파일의 크기가 4GB를 초과했습니다.");
                    }

                    if (File.Exists(filePath))
                    {
                        TJTarFile file = new TJTarFile();
                        file.Name = fileName;
                        file.RealFilePath = filePath;
                        file.IsNew = true;
                        file.Size = (uint)size;
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

                using (BinaryWriter outTJT = new BinaryWriter(new FileStream(outTJTFileName, FileMode.Create)))
                {

                    List<byte> tjtHeader = new List<byte>();
                    List<byte> tjtFileHeader = new List<byte>();
                    short crc = 0;

                    //Create TJT Header
                    //Version
                    tjtHeader.AddRange(Encoding.ASCII.GetBytes(this.Version.PadRight(0x0E, '\0')));
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
                        if (file.IsNew)
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

        public void RemoveFile(TJTarFile file)
        {

            //this.Files.Remove(file);

            int rmIdx = -1;
            for (int i = 0; i < this.Files.Count; i++)
            {
                if (this.Files[i].Index == file.Index)
                {
                    rmIdx = i;
                    break;
                }
            }

            if (rmIdx < 0)
            {
                throw new Exception($"File {file.Name} not found");
            }

            this.Files.RemoveAt(rmIdx);
            RealignFiles();
        }

        public void RemoveFiles(TJTarFile[] files)
        {
            int[] removeIndex = new int[files.Length];
            
            //초기화
            for(int i=0; i < removeIndex.Length; i++)
            {
                removeIndex[i] = -1;
            }
            
            //검색후 배열에 제거할 인덱스 저장
            for(int i = 0; i < files.Length; i++)
            {
                for(int j = 0; j < this.Files.Count; j++)
                {
                    if (files[i].Index == this.Files[j].Index)
                    {
                        removeIndex[i] = j;
                        break;
                    }
                }
            }

            Array.Sort(removeIndex);
            Array.Reverse(removeIndex);

            //제거
            for (int i = 0; i < removeIndex.Length; i++)
            {
                if (removeIndex[i] >= 0)
                {
                    this.Files.RemoveAt(removeIndex[i]);
                }
            }

            //재정렬
            RealignFiles();
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

        //TJ미디어의 찐빠로 가끔씩 4GB가 넘는 TJT파일이 나오는 경우가 생김
        //그럴 경우를 대비해서 64비트로 설정
        public ulong StartOffset;

        //새롭게 추가된 파일일때
        public string RealFilePath;

        //기존파일이면 True
        public bool IsNew;
    }

    //Custom Exception
    public class TJTarNotSupportedVersionException : Exception
    {
        public TJTarNotSupportedVersionException()
        {
        }

        public TJTarNotSupportedVersionException(string message)
            : base(message)
        {
        }

        public TJTarNotSupportedVersionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

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

    public class TJTarFileNameIsTooLongException : Exception
    {
        public TJTarFileNameIsTooLongException()
        {
        }

        public TJTarFileNameIsTooLongException(string message)
            : base(message)
        {
        }

        public TJTarFileNameIsTooLongException(string message, Exception inner)
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
