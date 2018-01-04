const app = getApp()
var list = require('../../util/list.js')
var ff = "getVRList"
Page({
  data: {
    pageIndex: 0,
    pageSize: 5,
    searchKeyword: 2,
    searchLoading: false,
    searchLoadingText: '正在加载数据...',
    url: '',
    list: []
  },
  onLoad: function () {
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