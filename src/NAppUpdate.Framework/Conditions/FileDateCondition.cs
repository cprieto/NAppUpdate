﻿using System;
using System.IO;
using NAppUpdate.Framework.Common;

namespace NAppUpdate.Framework.Conditions
{
    public class FileDateCondition : IUpdateCondition
    {
        public FileDateCondition()
        {
            Timestap = DateTime.MinValue;
        }

        [NauField("localPath",
            "The local path of the file to check. If not set but set under a FileUpdateTask, the LocalPath of the task will be used. Otherwise this condition will be ignored."
            , false)]
        public string LocalPath { get; set; }

        [NauField("timestamp", "Date-time to compare with", true)]
        public DateTime Timestap { get; set; }

        [NauField("what", "Comparison action to perform. Accepted values: newer, is, older. Default: older.", false)]
        public string ComparisonType { get; set; }

        #region IUpdateCondition Members

        public bool IsMet(Tasks.IUpdateTask task)
        {
            if (Timestap == DateTime.MinValue)
                return true;

            string localPath = !string.IsNullOrEmpty(LocalPath)
                                   ? LocalPath
                                   : Utils.Reflection.GetNauAttribute(task, "LocalPath") as string;
            if (string.IsNullOrEmpty(localPath) || !File.Exists(localPath))
                return true;

            DateTime localFileDateTime = File.GetLastWriteTime(localPath);
            switch (ComparisonType)
            {
                case "newer":
                    return localFileDateTime > Timestap;
                case "is":
                    return localFileDateTime.Equals(Timestap);
                default:
                    return localFileDateTime < Timestap; // == what="older"
            }
        }

        #endregion
    }
}
