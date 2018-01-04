var util = require('util.js')
const app = getApp()
function addPost(url, data) {
  wx.showNavigationBarLoading()
  wx.request({
    url: app.globalData.url + url,
    data: data,
    method: 'POST',
    header: {
      'Content-Type': 'application/x-www-form-urlencoded'
    },
    success: function (res) {
      if (res.data.result == 200) {
        wx.showToast({
          title: '成功',
          icon: 'success',
          duration: 2000
        })
      }
      else {
        request(url, data)
      }
    },
    fail: function (err) { request(url, data) },
    complete: function () {
      wx.hideNavigationBarLoading()
    }
  })
}
function request(url, data) {
  wx.showModal({
    title: '提示',
    content: '请求超时,是否重新请求?',
    success: function (res) {
      if (res.confirm) {
        addPost(url, data)
      }
    }
  })
}

module.exports = {
  addPost: addPost
}