const app = getApp()
var list = require('../../util/list.js')
var ff = "getOrderList"
Page({
  data: {
    pageIndex: 0,
    pageSize: 5,
    searchKeyword: 0,
    searchLoading: false,
    searchLoadingText: '正在加载数据...',
    url: '',
    list: []
  },
  onShow: function () {
    if (app.globalData.member_gid != "") {
      list.getDataList(ff, this)
    }
  },
  lower: function (e) {
    this.setData({
      searchLoadingText: '正在加载数据...'
    })
    list.getDataList(ff, this)
  }
})