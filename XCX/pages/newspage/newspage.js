var gid = require('../../util/gid.js')
var searchGid = "getNews"

Page({
  data: {
    searchKeyword:'',    
    url: '',
    searchGid: {},
    barTitle: ""
  },
  onLoad: function (options) {
    this.setData({
      searchKeyword: options.gid
    })
    gid.getGid(searchGid, this)
  },
  onShareAppMessage: function (res) {
    var that = this
    return {
      title: that.data.barTitle,
      path: '/pages/newspage/newspage?gid=' + that.data.searchKeyword
    }
  }
})