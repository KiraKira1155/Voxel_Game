using BlockDropItem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[System.Serializable]
public class BlockDropItemData
{
    public DropItem[] dropItem;
}

namespace BlockDropItem
{

    public class LoadData
    {
        private const string savePath = "/StreamingAssets/DropItem/Block/";

        public static string Load(EnumGameData.BlockID blockID)
        {
            var fs = new FileStream(
        Application.dataPath + savePath + blockID.ToString() + ".json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var reader = new StreamReader(fs, Encoding.UTF8);
            string json = reader.ReadToEnd();
            fs.Close();
            reader.Close();
            return json;
        }

        private static void CreateFile(EnumGameData.BlockID blockID)
        {
            var reader = Application.dataPath + savePath + blockID.ToString() + ".json";
            if (File.Exists(reader))
                File.Create(reader).Close();
        }

        /// <summary>
        /// �Z�[�u
        /// </summary>
        public static void Save(object instance, EnumGameData.BlockID blockID, bool overWrite = false)
        {
            CreateFile(blockID);
            var writer = new StreamWriter(Application.dataPath + savePath + blockID.ToString() + ".json", overWrite);
            string jsonstr = JsonUtility.ToJson(instance);
            writer.Write(jsonstr);
            writer.Flush();
            writer.Close();
        }

        public static bool CheckExistsFile(EnumGameData.BlockID blockID)
        {
            var reader = Application.dataPath + savePath + blockID.ToString() + ".json";
            return File.Exists(reader);
        }
    }

    [System.Serializable]
    public struct DropItem
    {
        public Roll roll;
        public ItemEntry[] entries;
        public MatchToolItemEntry[] matchToolEntry;
    }

    [System.Serializable]
    public struct ItemEntry
    {
        //ture�̏ꍇ�͑Ή������c�[���A�c�[�����x���ȏ�łȂ���΃h���b�v���Ȃ�
        public bool needToo;
        public bool perfectMiner;
        /// <summary>
        /// EnumGameData.ItemID
        /// </summary>
        public int dropItemID;
        //�G���g���[���ł̊m���A���v��100��
        public byte weight;
        //�h���b�v�m��
        public float probability;
        public int dropMinNum;
        public int dropMaxNum;
    }

    /// <summary>
    /// �ݒ肵���c�[���������ꍇ�͂������̃A�C�e�����h���b�v����
    /// </summary>
    [System.Serializable]
    public struct MatchToolItemEntry
    {
        /// <summary>
        /// EnumGameData.ItemID
        /// </summary>
        public int dropItemID;
        /// <summary>
        /// EnumGameData.ItemType
        /// </summary>
        public int toolType;
        //�h���b�v�m��
        public float probability;
        public int dropMinNum;
        public int dropMaxNum;
    }


    /// <summary>
    /// �G���g���[�����A�C�e���̒��I�񐔁A�����_��
    /// </summary>
    [System.Serializable]
    public struct Roll
    {
        public int min;
        public int max;
    }
}
