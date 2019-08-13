// import config from "@/config/index";
// 查询
const tableData = params => ({
    url: "/nsp-apis/uds/in/sales/v1/car-orders",
    method: "get",
    data: params
});
const orderStatus = {
    url: "/uds/ss/backend/v1/sales/ui/car/car_order_status",
    method: "get"
};
export default {
    tableData,
    orderStatus
};
