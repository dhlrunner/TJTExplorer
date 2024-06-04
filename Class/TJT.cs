using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJTExplorer.Class
{
    internal class TJT : IDisposable
    {
        public uint fileCount { get; private set; }
        public List<TJTFile> files { get; private set; }
        public string version { get; private set; }
        public string contentVersion { get; private set; }

        private const uint fileHeaderSize = 0x84;
        private BinaryReader tjtReader = null;
        private uint writerBufferSize = 4096;

        public TJT() 
        {
            fileCount = 0;
            files = new List<TJTFile>();
        }

        private uint calcCheckSum(byte[] data, int dataSize)
        {
            uint dec_checksum = 0;
            for (int i = 0; dataSize > i; ++i)
                dec_checksum += data[i];

            return dec_checksum;
        }

        public void loadFromFile(string filename)
        {
            tjtReader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            this.version = Encoding.ASCII.GetString(tjtReader.ReadBytes(0x10)).Split('\0')[0];
            uint unk = tjtReader.ReadUInt32();
            this.fileCount = tjtReader.ReadUInt32();
            uint crc = tjtReader.ReadUInt32();

            //uint dec_checksum = 0;
            //while (tjtReader.BaseStream.Position < tjtReader.BaseStream.Length)
            //{
            //    byte[] b = tjtReader.ReadBytes(4096);

            //    for (int i = 0; b.Length > i; ++i)
            //        dec_checksum += b[i];
            //}

            this.contentVersion = Encoding.ASCII.GetString(tjtReader.ReadBytes(0x10)).Split('\0')[0];


            //TJT헤더크기 + (파일헤더크기 * 파일개수)
            uint dataStartOffset = ((this.fileCount) * fileHeaderSize) + 0x2C;

            uint lastFileSize = 0;
            for (uint i = 0; i < this.fileCount; i++)
            {
                string fname = Encoding.ASCII.GetString(tjtReader.ReadBytes(0x80)).Split('\0')[0];
                uint fsize = tjtReader.ReadUInt32();
                uint fIdx = i;
                uint fOffset = dataStartOffset + lastFileSize;

                lastFileSize = lastFileSize + fsize;

                var fileInfo = new TJTFile();
                fileInfo.fileName = fname;
                fileInfo.fileSize = fsize;
                fileInfo.fileIndex = fIdx;
                fileInfo.fileStartOffset = fOffset;

                files.Add(fileInfo);
            }
        }

        public TJTFile getFileHeaderByIndex(uint fileIndex)
        {
            var res = this.files.Where(n => n.fileIndex == fileIndex);
            return res.First();
        }

        public async Task extractFileAsync(TJTFile file, string outFileName)
        {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(outFileName, FileMode.Create)))
            {
                await Task.Run(() => {
                    this.tjtReader.BaseStream.Seek(file.fileStartOffset, SeekOrigin.Begin);

                    uint amari = file.fileSize % this.writerBufferSize;
                    uint kake = file.fileSize / this.writerBufferSize;

                    //Console.WriteLine($"amari {amari} kake {kake}");

                    for (int i = 0; i < kake; i++)
                    {
                        byte[] _buffer = this.tjtReader.ReadBytes((int)this.writerBufferSize);
                        writer.Write(_buffer);
                    }

                    byte[] buffer = this.tjtReader.ReadBytes((int)amari);
                    writer.Write(buffer);
                    writer.Flush();
                });
               
            }
        }

        public byte[] getFileBytes(TJTFile file)
        {
            this.tjtReader.BaseStream.Seek(file.fileStartOffset, SeekOrigin.Begin);
            return this.tjtReader.ReadBytes((int)file.fileSize);
        }

        public virtual void Dispose()
        {
            this.tjtReader.Dispose();
        }
    }

    internal struct TJTFile
    {
        public string fileName;
        public uint fileSize;
        public uint fileIndex;
        public uint fileStartOffset;
    }
}
