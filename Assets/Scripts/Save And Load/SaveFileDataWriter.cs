using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace SG
{
    public class SaveFileDataWriter
    {
        public string saveDataDirectoryPath = "";
        public string saveFileName = "";

        // Kiểm tra file có tồn tại hay ko
        public bool CheckToSeeIfFileExists()
        {
            if(File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Xóa file save
        public void DeleteSaveFile()
        {
            File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
        }

        //Used to creater a save file upon starting a new game
        public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
        {
            // Tạo đường dẫn để lưu file (vị trí trên máy)
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                // Tạo thư mục mà tập tin sẽ được ghi vào, nếu nó chưa tồn tại
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("Create save file, at save path: " + savePath);

                // tuần tự hóa đối tượng dữ liệu game C# thành json
                string dataToStore = JsonUtility.ToJson(characterData, true);

                // Ghi file vào hệ thống
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWrite = new StreamWriter(stream))
                    {
                        fileWrite.Write(dataToStore);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LỖI KHI CỐ GẮNG LƯU DỮ LIỆU NHÂN VẬT, TRÒ CHƠI KHÔNG LƯU" + savePath + "\n" + ex);
            }
        }

        // Used to load a save file upon loading a previous game
        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterData = null;

            // Tạo đường dẫn để Load file (vị trí trên máy)
            string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

            if(File.Exists(loadPath))
            {
                try
                {
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // Giải tuần tự hóa dữ liệu từ json trở lại Unity
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch(Exception ex)
                {
                    Debug.Log("bla bla");
                }

            }
            return characterData;
        }
    }
}