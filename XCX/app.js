App({
  onLaunch: function () {
  },
  onShow: function () {
    var that = this
    //用户信息
    wx.checkSession({
      success: function () {
        var value = wx.getStorageSync('member')
        if (value) {
          that.globalData.member_gid = value.member_gid
          that.globalData.login_identifier = value.login_identifier
        }
      }
    })
  },
  ok: function () {
    wx.showToast({
      title: '成功!',
      icon: 'success',
      duration: 3000
    })
  },
  err: function () {
    wx.showToast({
      title: '失败!',
      image: '/image/x.png',
      duration: 3000
    })
  },
  showLoading: function () {
    wx.showLoading({
      title: '加载中...',
      mask: true,
    })
  },
  loginPage: function () {
    wx.switchTab({
      url: '/pages/member/member'
    })
  },
  loginout: function () {
    wx.clearStorageSync()
    wx.clearStorage()
    this.globalData.userInfo = null
    this.globalData.member_gid = ""
    this.globalData.login_identifier = ""
  },
  globalData: {
    url: 'https://www.fpzs110.com/ajax/api.ashx?ff=',
    member_gid: '',
    login_identifier: ''
  }
})