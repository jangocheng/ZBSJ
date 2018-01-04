var list = require('../../util/list.js')
var ff = "getNewsList"
Page({
  data: {
    pageIndex: 0,
    pageSize: 5,
    searchKeyword: '',
    searchLoading: false,
    searchLoadingText: '正在加载数据...',
    url: '',
    list: []
  },
  onLoad: function () {
    list.getDataList(ff, this)
  },
  onPullDownRefresh: function () {
    this.setData({
      pageIndex: 0,
      list: []
    })
    list.getDataList(ff, this)
  },
  bindKeywordInput: function (e) {
    this.setData({
      searchKeyword: e.detail.value
    })
  },
  keywordSearch: function (e) {
    this.setData({
      pageIndex: 0,
      list: [],
      searchLoadingText: '正在搜索数据...'
    })
    list.getDataList(ff, this)
  },
  lower: function (e) {
    this.setData({
      searchLoadingText: '正在加载数据...'
    })
    list.getDataList(ff, this)
  },
  onShareAppMessage: function (res) {
    return {
      title: '真柏资讯',
      path: '/pages/news/news'
    }
  }
})