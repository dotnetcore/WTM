// const bodyParser = require('body-parser');
const Mock = require('mockjs');
const lodash = require('lodash');
module.exports = (app) => {
    // app.use(bodyParser.json());
    // login(app);
    // search(app);
    select(app);
    // remove(app);
};
const { dataSource, selectList, treeSelect } = createData();
// function login(app) {
//     const loginToken = new Map();
//     app.post('/mock/login', function (req, res) {
//         console.log("/mock/login", req.body)
//         const { itcode, password } = req.body;
//         if (String(password) !== '000000') {
//             return setTimeout(() => {
//                 res.status(400)
//                 res.send({
//                     code: 400,
//                     msg: '密码错误'
//                 })
//             }, 500)
//         }
//         const token = Mock.Random.guid();
//         const data = {
//             code: 200,
//             msg: 'msg',
//             data: {
//                 itcode,
//                 token,
//             }
//         }
//         loginToken.set(token, data);
//         setTimeout(() => {
//             res.send(data)
//         }, 500)
//     });
//     app.get('/mock/checkLogin/:token', function (req, res) {
//         console.log("/mock/checkLogin", req.params)
//         const { token } = req.params;
//         if (loginToken.has(token)) {
//             setTimeout(() => {
//                 res.send(loginToken.get(token))
//             }, 500)
//         } else {
//             setTimeout(() => {
//                 res.status(401)
//                 res.send({
//                     code: 401,
//                     msg: '认证失败'
//                 })
//             }, 500)
//         }
//     })
//     app.get('/mock/logout/:token', function (req, res) {
//         console.log("/mock/logout", req.params)
//         const { token } = req.params;
//         loginToken.delete(token)
//         res.send({})
//     })

// }
// /**
//  * 搜索
//  * @param {*} app 
//  */
// function search(app) {
//     app.post('/mock/search', function (req, res) {
//         console.log("/mock/search", req.body)
//         const { current = 1, pageSize = 10 } = req.body.pagination;
//         const data = Mock.mock({
//             code: 200,
//             msg: 'msg',
//             data: {
//                 total: dataSource.length, // 数据总数
//                 pageSize: pageSize, // 每页条数
//                 current: current, //当前页码
//                 dataSource: lodash.get(lodash.chunk(dataSource, pageSize), `[${current - 1}]`, [])
//             }
//         })
//         setTimeout(() => {
//             res.send(data)
//         }, 500)
//     });
// }
// /**
//  * 删除 
//  * @param {*} app 
//  */
// function remove(app) {
//     app.post('/mock/remove', function (req, res) {
//         console.log("/mock/remove", req.body);
//         const { itcodes = [] } = req.body;
//         const removes = lodash.remove(dataSource, x => itcodes.some(itcode => itcode === x.itcode));
//         setTimeout(() => {
//             res.send({
//                 code: 200,
//                 msg: '删除成功',
//                 removes: removes
//             })
//         }, 200)
//     });
// }
/**
 * 级联列表
 * @param {*} app 
 */
function select(app) {
    app.get('/mock/select', function (req, res) {
        console.log("/mock/select", req.query)
        const { one, two } = req.query;
        let data = [];
        // 三级
        if (one && two) {
            data = lodash.get(lodash.find(selectList, ['Value', one]), 'children', []);
            data = lodash.get(lodash.find(data, ['Value', two]), 'children', []);
        }
        // 二级
        else if (one) {
            data = lodash.get(lodash.find(selectList, ['Value', one]), 'children', []);
        }
        // 一级
        else {
            data = selectList
        }
        setTimeout(() => {
            // res.send({
            //     code: 200,
            //     msg: 'msg',
            //     data: lodash.uniqBy(data, 'value')
            // })
            res.send(lodash.uniqBy(data, 'Value'))
        }, 500);
    })
    app.get('/mock/treeSelect', function (req, res) {
        console.log("/mock/treeSelect", req.query)
        setTimeout(() => {
            // res.send({
            //     code: 200,
            //     msg: 'msg',
            //     data: treeSelect
            // })
            res.send(treeSelect)
        }, 500);
    })

}

function createData() {
    // 级联数据
    const { selectList, treeSelect } = Mock.mock({
        'selectList|20': [
            {
                Text: '@province',
                Value: function () {
                    return this.Text
                },
                'children|20': [
                    {
                        Text: '@city',
                        Value: function () {
                            return this.Text
                        },
                        'children|20': [
                            {
                                Text: '@county',
                                Value: function () {
                                    return this.Text
                                },
                            }
                        ]
                    }
                ]
            }
        ],
        'treeSelect|20': [
            {
                title: '@province',
                value: function () {
                    return Mock.Random.guid()
                },
                key: function () {
                    return this.value
                },
                'children|20': [
                    {
                        title: '@city',
                        value: function () {
                            return Mock.Random.guid()
                        },
                        key: function () {
                            return this.value
                        },
                        'children|20': [
                            {
                                title: '@county',
                                value: function () {
                                    return Mock.Random.guid()
                                },
                                key: function () {
                                    return this.value
                                },
                            }
                        ]
                    }
                ]
            }
        ],
    });
    // 表格数据
    const { dataSource } = Mock.mock({
        'dataSource|999': [
            {
                'itcode': '@guid',
                'name|+1': '@cname',
                'sex|1': ['男', '女'],
                'age|18-30': 30,
                'province': function () {
                    return lodash.get(selectList, `[${lodash.random(0, 19)}].value`);
                },
                'city': function () {
                    const province = lodash.findIndex(selectList, ['value', this.province]);
                    return lodash.get(selectList, `[${province}].children[${lodash.random(0, 19)}].value`);
                },
                'county': function () {
                    const province = lodash.findIndex(selectList, ['value', this.province]);
                    const city = lodash.findIndex(lodash.get(selectList, `[${province}].children`, []), ['value', this.city]);
                    return lodash.get(selectList, `[${province}].children[${city}].children[${lodash.random(0, 19)}].value`);
                },
                'avatar': function () {
                    return Mock.Random.image('100x100', Mock.mock('@color'), Mock.mock('@color'), 'Mock.js');
                },
                'introduce': '@cparagraph(1, 3)'
            }
        ]
    });
    return {
        selectList,
        treeSelect,
        dataSource
    }
}