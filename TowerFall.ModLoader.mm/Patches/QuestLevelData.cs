using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerFall
{
    class patch_QuestLevelData : QuestLevelData
    {
        public patch_QuestLevelData() : base(0, null)
        {

        }

        public QuestLevelData DeepClone()
        {
            QuestLevelData New = (QuestLevelData)MemberwiseClone();
            New.DataPath = string.Copy(DataPath);
            New.Path = string.Copy(Path);
            return New;
        }
    }
}
