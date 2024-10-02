using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace CustomUtility
{
    namespace IO
    {
        /// <summary>
        /// # Summary
        /// A concrete implementation of `SaveHandle` for handling binary save operations.
        /// This class provides methods to save and load data in a binary format using Unity's JsonUtility and .NET's BinaryFormatter.
        /// `SaveHandle`의 구체적인 구현으로, 바이너리 저장 작업을 처리한다.
        /// 이 클래스는 Unity의 JsonUtility와 .NET의 BinaryFormatter를 사용하여 데이터를 바이너리 형식으로 저장하고 불러오는 메서드를 제공한다.
        ///
        /// ## Usage:
        /// - Use the `Save` method to serialize and save data objects to a binary file.
        /// - Use the `Load` method to deserialize and load data objects from a binary file.
        /// ## 사용 방법:
        /// - `Save` 메서드를 사용하여 데이터 객체를 직렬화하고 바이너리 파일로 저장한다.
        /// - `Load` 메서드를 사용하여 바이너리 파일에서 데이터 객체를 역직렬화하고 불러온다.
        ///
        /// ## Features:
        /// - Automatically creates the necessary directories for saving files.
        /// - Converts data to a binary format for efficient storage.
        /// - Ensures that the binary data is valid and the file is accessible before saving or loading.
        /// ## 기능:
        /// - 파일을 저장하기 위한 필요한 디렉토리를 자동으로 생성한다.
        /// - 데이터를 효율적으로 저장하기 위해 바이너리 형식으로 변환한다.
        /// - 저장 또는 로드하기 전에 바이너리 데이터가 유효하고 파일에 접근할 수 있는지 확인한다.
        /// </summary>
        public class BinarySaveHandler : SaveHandle
        {
            /// <summary>
            /// Saves the provided data object as a binary file.
            /// 주어진 데이터 객체를 바이너리 파일로 저장한다.
            /// </summary>
            /// <typeparam name="T">The type of data to save. 저장할 데이터의 타입.</typeparam>
            /// <param name="target">The data object to save. 저장할 데이터 객체.</param>
            public override void Save<T>(T target)
            {
                Directory.CreateDirectory(BasePath);
                
                string filePath = GetFilePath(target.FileName);
                string jsonString = JsonUtility.ToJson(target);
                
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    byte[] bytes = Encoding.Default.GetBytes(jsonString);
                    
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, bytes);
                }
                
                IsFileAccessible(filePath);
            }

            /// <summary>
            /// Loads data from a binary file into the provided data object.
            /// 바이너리 파일에서 데이터를 불러와 제공된 데이터 객체에 저장한다.
            /// </summary>
            /// <typeparam name="T">The type of data to load. 불러올 데이터의 타입.</typeparam>
            /// <param name="target">The data object to load into. 데이터를 불러올 객체.</param>
            public override void Load<T>(ref T target)
            {
                string filePath = GetFilePath(target.GetType().ToString());
                if (!IsFileAccessible(filePath)) return;
                
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    byte[] bytes = (byte[])bf.Deserialize(fs);
                    
                    string jsonString = Encoding.Default.GetString(bytes);

                    if (IsFileEmpty(jsonString, SaveType.BINARY)) return;

                    target = JsonUtility.FromJson<T>(jsonString);
                }
            }
        }
    }
}
