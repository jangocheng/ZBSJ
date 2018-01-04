const app = getApp()
function login(that) {
  wx.checkSession({
    success: function () {
      var value = wx.getStorageSync('member')
      that.setData({
        member_gid: value.member_gid,
        login_identifier: value.login_identifier
      })
    },
    fail: function () {
      loginout(that)
    }
  })
}

function loginPage() {
  wx.showToast({
    title: '请先登录',
    icon: 'success',
    duration: 3000,
    success: function (res) {
      wx.switchTab({
        url: '/pages/member/member'
      })
    }
  })
}

function loginout(that) {
  app.globalData.userInfo = null
  wx.removeStorageSync({
    key: 'member',
    fail: function (res) {
      wx.clearStorageSync()
      wx.clearStorage()
    }
  })
  that.setData({
    member_gid: "",
    login_identifier:""
  })
}

module.exports = {
  login: login,
  loginPage: loginPage
}