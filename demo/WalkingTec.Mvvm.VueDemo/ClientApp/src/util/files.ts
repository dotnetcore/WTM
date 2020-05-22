/**
 * 创建二进制文件
 * @param response
 */
const createBlob = (
    response
) => {
    const blob = response.data;
    const fileName = response.headers['content-disposition'].match(/filename=(.*?);/)[1];
    const a = document.createElement("a");
    const downUrl = window.URL.createObjectURL(blob);
    a.href = downUrl;
    a.download = fileName;
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
