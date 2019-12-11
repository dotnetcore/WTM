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
                FileAttachment del = new FileAttachment { ID = Entity.ID };
                DC.Set<FileAttachment>().Attach(del);
                DC.Set<FileAttachment>().Remove(del);
                DC.SaveChanges();
            }
            catch (DbUpdateException)
            {
                MSD.AddModelError("", Program._localizer["DataCannotDelete"]);
            }
        }
    }
}
