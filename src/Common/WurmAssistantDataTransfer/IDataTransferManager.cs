using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistantDataTransfer
{
    public interface IDataTransferManager
    {
        /// <summary>
        /// </summary>
        /// <param name="filePath">File must no exist</param>
        /// <param name="dto"></param>
        void SaveToFile(string filePath, WurmAssistantDto dto);

        /// <summary>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        WurmAssistantDto LoadFromFile(string filePath);
    }
}