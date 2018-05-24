using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    public class FileAttachmentVM : BaseCRUDVM<FileAttachment>
    {
        public override void DoDelete()
        {
            try
            {
                if (Entity.SaveFileMode == SaveFileModeEnum.Local && !string.IsNullOrEmpty(Entity.Path))
                {
                    Utils.DeleteFile(Entity.Path);
                }
                DC.Database.ExecuteSqlCommand(new RawSqlString($"delete from {DC.GetTableName<FileAttachment>()} where ID='{Entity.ID}'"));
            }
            catch (DbUpdateException)
            {
                MSD.AddModelError("", "数据被使用，无法删除");
            }
        }
    }
}
