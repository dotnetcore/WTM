/**
 * 创建二进制文件
 * @param response
 */
const createBlob = (
    response,
    fileName: string | number = new Date().getTime(),
    fileType = ".xls"
) => {
    const blob = response;
    const a = document.createElement("a");
    const downUrl = window.URL.createObjectURL(blob);
    a.href = downUrl;
    switch (blob.type) {
    case "application/vnd.ms-excel":
        a.download = fileName + ".xls";
        break;
    default:
        a.download = fileName + fileType;
        break;
    }
    a.addEventListener(
        "click",
        () => {
            setTimeout(() => {
                window.URL.revokeObjectURL(downUrl);
            }, 1000);
        },
        false
    );
    a.click();
};
export { createBlob };
