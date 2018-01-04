const app = getApp()

Page({
  data: {
    userInfo: {},
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo'),
    video: ""
  },
  onShow: function () {
    if (app.globalData.member_gid == "") {
      this.out()
      wx.showToast({
        title: '请先登录',
        image: '/image/x.png',
        duration: 2000
      })
    }
    else {
      // 获取用户信息
      wx.getSetting({
        success: res => {
          if (res.authSetting['scope.userInfo']) {
            // 已经授权，可以直接调用 getUserInfo 获取头像昵称，不会弹框
            wx.getUserInfo({
              success: res => {
                // 可以将 res 发送给后台解码出 unionId
                this.setData({
                  userInfo: res.userInfo,
                  hasUserInfo: true
                })
                // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
                // 所以此处加入 callback 以防止这种情况
                if (this.userInfoReadyCallback) {
                  this.userInfoReadyCallback(res)
                }
              }
            })
          }
        }
      })
    }
    getVideo(this)
  },
  getUserInfo: function (e) {
    app.showLoading()
    var that = this
    // 登录
    wx.login({
      success: res => {
        // 发送 res.code 到后台换取 openId, sessionKey, unionId
        wx.request({
          url: app.globalData.url + 'oauth',
          data: {
            code: res.code,
            nickName: e.detail.userInfo.nickName,
            avatarUrl: e.detail.userInfo.avatarUrl,
            gender: e.detail.userInfo.gender,
            province: e.detail.userInfo.province,
            city: e.detail.userInfo.city
          },
          method: 'POST',
          header: {
            'Content-Type': 'application/x-www-form-urlencoded'
          },
          success: function (res) {
            if (res.data.result == 200) {
              app.globalData.member_gid = res.data.data.gid
              app.globalData.login_identifier = res.data.data.login_identifier
              wx.setStorageSync('member', {
                member_gid: res.data.data.gid,
                login_identifier: res.data.data.login_identifier
              })
              that.setData({
                userInfo: e.detail.userInfo,
                hasUserInfo: true
              })
              getVideo(that)
            }
            else {
              that.loginerr()
            }
          },
          fail: function (err) { that.loginerr() },//请求失败  
          complete: function () { wx.hideLoading() }//请求完成后执行的函数 
        })
      }
    })
  },
  loginerr: function () {
    var that = this
    wx.showModal({
      title: '登录失败',
      content: '登录异常,请完全退出小程序在登录尝试一下!',
      showCancel: false,
      confirmText: '我知道了',
      success: function (res) {
        that.out()
      }
    })
  },
  out: function () {
    this.setData({
      hasUserInfo: false,
      canIUse: true,
      video: "登录失效,请重新登录!"
    })
    app.loginout()
    //console.log(this.data)
  }
})

function getVideo(that)
{
  wx.request({
    url: app.globalData.url + 'MVideo',
    data: {
      member_gid: app.globalData.member_gid,
      login_identifier: app.globalData.login_identifier,
      searchKeyword: app.globalData.member_gid
    },
    method: 'POST',
    header: {
      'Content-Type': 'application/x-www-form-urlencoded'
    },
    success: function (res) {
      //console.log(res)
      if (res.data.result == 200) {
        if (res.data.data.type == 1) {
          that.setData({
            video: "你是VIP,可免费观看所有视频"
          })
        }
        else if (res.data.data.type == 2) {
          that.setData({
            video: "你可观看的视频个数:" + res.data.data.number
          })
        }
        else {
          that.setData({
            video: "你没有可观看的视频个数"
          })
        }
      }
      else
      {
        that.out()
      }
    },
    fail: function (err) { 
      that.setData({
        video: "登录失效,请重新登录!"
      })
      app.err() 
      }, 
    complete: function () { }//请求完成后执行的函数 
  })
}