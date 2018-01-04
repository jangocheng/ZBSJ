const app = getApp()
Page({
  data: {
    url:'',
    xcp:{},
    video:[]
  },
  onPullDownRefresh:function()
  {
    this.getDataList()
  },
  onLoad: function () {
    this.getDataList()
  },
  getDataList:function()
  {
    wx.showNavigationBarLoading()
    var that = this
    wx.request({
      url: app.globalData.url + 'getIndex',
      data: {
      },
      method: 'POST',
      header: {
        'Content-Type': 'application/x-www-form-urlencoded'
      },
      success: function (res) {
        if (res.data.result == 200) {
          that.setData({
            url: res.data.data.url,
            xcp: res.data.data.xcp,
            video: res.data.data.video
          })
        }
        else {
          that.request()
        }
      },
      fail: function (err) { that.request() },
      complete: function () {
        wx.stopPullDownRefresh()
        wx.hideNavigationBarLoading()
       }
    })
  },
  request:function()
  {
    var that = this
    wx.showModal({
      title: '提示',
      content: '请求超时,是否重新请求?',
      success: function (res) {
        if (res.confirm) {
          that.getDataList()
        }
      }
    })
  },
  onShareAppMessage: function (res) {
    if (res.from === 'button') {
      // 来自页面内转发按钮
      console.log(res.target)
    }
    return {
      title: '真柏世界',
      path: '/pages/index/index',
      success: function (res) {
        // 转发成功
      },
      fail: function (res) {
        // 转发失败
      }
    }
  }
})