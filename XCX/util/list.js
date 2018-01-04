var util = require('util.js')
const app = getApp()
function getDataList(url, that) {
  wx.showNavigationBarLoading()
  that.setData({
    searchLoading: true
  })
  var pageIndex = that.data.pageIndex + 1
  wx.request({
    url: app.globalData.url + url,
    data: {
      pageIndex: pageIndex,
      pageSize: that.data.pageSize,
      searchKeyword: that.data.searchKeyword,
      member_gid: app.globalData.member_gid,
      login_identifier: app.globalData.login_identifier
    },
    method: 'POST',
    header: {
      'Content-Type': 'application/x-www-form-urlencoded'
    },
    success: function (res) {
      if (res.data.result == 200) {
        if (res.data.data.list.length == 0) {
          that.setData({
            searchLoading: true,
            searchLoadingText: util.dateFtt("yyyy-MM-dd hh:mm:ss", new Date()) + ' 已加载全部'
          })
        }
        else {
          that.setData({
            searchLoading: false,
            pageIndex: pageIndex,
            url: res.data.data.url,
            list: that.data.list.concat(res.data.data.list)
          })
        }
      }
      else {
        request(url, that)
      }
    },
    fail: function (err) { request(url, that) },
    complete: function () {
      wx.stopPullDownRefresh()
      wx.hideNavigationBarLoading()
    }
  })
}
function request(url, that) {
  that.setData({
    pageIndex: pageIndex - 1
  })
  wx.showModal({
    title: '提示',
    content: '请求超时,是否重新请求?',
    success: function (res) {
      if (res.confirm) {
        getDataList(url, that)
      }
    }
  })
}

module.exports = {
  getDataList: getDataList,
  request: request
}