var util = require('util.js')
const app = getApp()
function getGid(url, that) {
  wx.showNavigationBarLoading()
  wx.request({
    url: app.globalData.url + url,
    data: {
      searchKeyword: that.data.searchKeyword
    },
    method: 'POST',
    header: {
      'Content-Type': 'application/x-www-form-urlencoded'
    },
    success: function (res) {
      //console.log(res)
      if (res.data.result == 200) {
        that.setData({
          url: res.data.data.url,
          barTitle: res.data.data.list.name,
          searchGid: res.data.data.list
        })
        if (url == "getVideo") {
          that.setData({
            price: res.data.data.list.price,
            pricegid: res.data.data.list.gid,
            tprice: res.data.data.list.tprice.subtitle,
            vip: res.data.data.list.vip.subtitle,
            tpricegid: res.data.data.list.tprice.gid,
            vipgid: res.data.data.list.vip.gid
          })
        }
        wx.setNavigationBarTitle({
          title: res.data.data.list.name
        })
      }
      else {
        request(url, that)
      }
    },
    fail: function (err) { request(url, that) },
    complete: function () {
      wx.hideNavigationBarLoading()
    }
  })
}
function request(url, that) {
  wx.showModal({
    title: '提示',
    content: '请求超时,是否重新请求?',
    success: function (res) {
      if (res.confirm) {
        getGid(url, that)
      }
    }
  })
}

module.exports = {
  getGid: getGid,
  request: request
}