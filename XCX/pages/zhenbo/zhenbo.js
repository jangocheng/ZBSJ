const app = getApp()
var list = require('../../util/list.js')
var ff = "getOrderList"
Page({
  data: {
    odList: [],
    kdList: [],
    showKD: 'hide',
    showOrder: 'hide',
    pageIndex: 0,
    pageSize: 3,
    searchKeyword: 1,
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
  },
  showOrder: function (event) {
    var that = this
    that.setData({
      showOrder: 'show'
    })
    wx.request({
      url: app.globalData.url + 'getODList',
      data: {
        order_gid: event.currentTarget.dataset.buy
      },
      method: 'POST',
      header: {
        'Content-Type': 'application/x-www-form-urlencoded'
      },
      success: function (res) {
        if (res.data.result == 200) {
          that.setData({
            odList: res.data.data.list
          })
        }
      },
      fail: function (err) { app.err() },//请求失败  
      complete: function () { }//请求完成后执行的函数 
    })
  },
  showKD: function (event) {
    if (event.currentTarget.dataset.express_number != null && event.currentTarget.dataset.express != null) {
      var that = this
      that.setData({
        showKD: 'show'
      })
      wx.request({
        url: app.globalData.url + 'kd',
        data: {
          express_number: event.currentTarget.dataset.express_number,
          express: event.currentTarget.dataset.express
        },
        method: 'POST',
        header: {
          'Content-Type': 'application/json'
        },
        success: function (res) {
          console.log(res)
          if (res.data.result == 200) {
            that.setData({
              kdList: res.data.data.list
            })
          }
        },
        fail: function (err) { app.err() },//请求失败  
        complete: function () { }//请求完成后执行的函数 
      })
    }
    else {
      wx.showToast({
        title: '没有物流信息',
        icon: 'success',
        duration: 2000
      })
    }
  },
  hide: function () {
    this.setData({
      showOrder: 'hide',
      showKD: 'hide',
      kdList: [],
      odList: []
    })
  }
})