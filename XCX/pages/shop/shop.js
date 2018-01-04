var list = require('../../util/list.js')
var ff = "getShopList"

Page({
  data: {
    currentTab: 0, //预设当前项的值
    scrollLeft: 0, //tab标题的滚动条位置
    pageIndex: 0,
    pageSize: 9,
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
  // 滚动切换标签样式
  switchTab: function (e) {
    this.setData({
      currentTab: e.detail.current
    });
    this.checkCor();
  },
  // 点击标题切换当前页时改变样式
  swichNav: function (e) {
    var cur = e.target.dataset.current;
    if (this.data.currentTaB == cur) { return false; }
    else {
      this.setData({
        currentTab: cur
      })
    }
  },
  //判断当前滚动超过一屏时，设置tab标题滚动条。
  checkCor: function () {
    if (this.data.currentTab > 4) {
      this.setData({
        scrollLeft: 300
      })
    } else {
      this.setData({
        scrollLeft: 0
      })
    }
  },
  bindKeywordInput: function (e) {
    this.setData({
      searchKeyword: e.detail.value
    })
  },
  lower: function (e) {
    this.setData({
      searchLoadingText: '正在加载数据...'
    })
    list.getDataList(ff, this)
  },
  onShareAppMessage: function (res) {
    return {
      title: '真柏商城',
      path: '/pages/shop/shop'
    }
  }
})