const app = getApp()
var list = require('../../util/list.js')
var gid = require('../../util/gid.js')
var add = require('../../util/add.js')
var ff = "getCommentList"
var searchGid = "getVideo"
var addComment = "addComment"

Page({
  data: {
    pageIndex: 0,
    pageSize: 5,
    searchKeyword: '',
    searchLoading: false,
    searchLoadingText: '正在加载数据...',
    url: '',
    list: [],
    searchGid: {},
    barTitle: "",
    plhide: false,
    focus: false,
    price: '',
    tprice: '',
    vip: '',
    pricegid: '',
    tpricegid: '',
    vipgid: '',
    play: false
  },
  onLoad: function (options) {
    this.setData({
      searchKeyword: options.gid
    })
    gid.getGid(searchGid, this)
    list.getDataList(ff, this)
  },
  onReady: function (res) {
    this.videoContext = wx.createVideoContext('video')
  },
  onShareAppMessage: function (res) {
    var that = this
    return {
      title: that.data.barTitle,
      path: '/pages/videoplay/videoplay?gid=' + that.data.searchKeyword
    }
  },
  pl: function (e) {
    this.setData({
      plhide: true,
      focus: true
    })
  },
  qxpl: function (e) {
    this.setData({ plhide: false });
  },
  formBindsubmit: function (e) {
    if (e.detail.value.content.length < 3) {
      wx.showToast({
        title: '评论不能少于3个字',
        icon: 'success',
        duration: 2000
      })
    } else {
      if (app.globalData.member_gid != "") {
        var data = {
          member_gid: app.globalData.member_gid,
          product_gid: this.data.searchKeyword,
          content: e.detail.value.content
        }
        add.addPost(addComment, data)
        this.setData({
          plhide: false,
          pageIndex: 0,
          list: []
        })
        list.getDataList(ff, this)
      }
      else {
        app.loginPage()
      }
    }
  },
  lower: function (e) {
    this.setData({
      searchLoadingText: '正在加载数据...'
    })
    list.getDataList(ff, this)
  },
  bindplay: function (e) {
    var that = this
    if (that.data.price > 0) {
      if (!that.data.play) {
        app.showLoading()
        that.videoContext.pause()
        if (app.globalData.member_gid != "") {
          wx.request({
            url: app.globalData.url + "MVideo",
            data: {
              searchKeyword: that.data.searchKeyword,
              member_gid: app.globalData.member_gid,
              login_identifier: app.globalData.login_identifier
            },
            method: 'POST',
            header: {
              'Content-Type': 'application/x-www-form-urlencoded'
            },
            success: function (res) {
              //console.log(res)
              if (res.data.result == 200) {
                var type = res.data.data.type
                if (type == 1) {
                  that.setData({
                    play: true
                  })
                  that.videoContext.play()
                  wx.hideLoading()
                }
                else if (type == 2) {
                  playVideo("你还可观看" + res.data.data.number + "个视频,确定消耗观看吗?", type, that)
                }
                else if (type == 3) {
                  playVideo("你还可观看" + res.data.data.number + "次该视频,确定消耗一次机会观看吗?", type, that)
                }
                else {
                  buyVideo(that)
                }
              }
              else {
                wx.showToast({
                  title: res.data.data,
                  icon: 'success',
                  duration: 2000
                })
              }
            },
            fail: function (err) { request(url, that) },
            complete: function () {
              wx.hideLoading()
            }
          })
        }
        else {
          app.loginPage()
        }
      }
    }
  }
})

function playVideo(content, type, that) {
  wx.hideLoading()
  wx.showModal({
    title: '提示',
    content: content,
    success: function (res) {
      if (res.confirm) {
        app.showLoading()
        wx.request({
          url: app.globalData.url + "videoNumber",
          data: {
            searchKeyword: that.data.searchKeyword,
            member_gid: app.globalData.member_gid,
            login_identifier: app.globalData.login_identifier,
            type: type
          },
          method: 'POST',
          header: {
            'Content-Type': 'application/x-www-form-urlencoded'
          },
          success: function (res) {
            //console.log(res)
            that.videoContext = wx.createVideoContext('video')
            if (res.data.result == 200) {
              that.setData({
                play: true
              })
              that.videoContext.play()
            }
            else {
              wx.showToast({
                title: res.data.data,
                icon: 'success',
                duration: 2000
              })
            }
          },
          fail: function (err) { app.err() },
          complete: function () {
            wx.hideLoading()
          }
        })
      }
    }
  })
}

function buyVideo(that) {
  wx.hideLoading()
  wx.showActionSheet({
    itemList: ['观看当前视频 ' + that.data.price + " 元/" + that.data.searchGid.extend4 + "次", that.data.tprice, that.data.vip],
    success: function (res) {
      var product_gid = ""
      if (res.tapIndex == 0) {
        product_gid = that.data.pricegid
      }
      else if (res.tapIndex == 1) {
        product_gid = that.data.tpricegid
      }
      else if (res.tapIndex == 2) {
        product_gid = that.data.vipgid
      }
      else {
        product_gid = ""
      }
      if (product_gid != "" && app.globalData.member_gid != "") {
        app.showLoading()
        wx.request({
          url: app.globalData.url + "order",
          data: {
            searchKeyword: that.data.searchKeyword,
            product: [{ product_gid: product_gid, number: 1 }],
            member_gid: app.globalData.member_gid,
            login_identifier: app.globalData.login_identifier,
            type: 2,
            contact_number: '',
            consignee: '',
            address: '',
            remarks: ''
          },
          method: 'POST',
          header: {
            'Content-Type': 'application/json'
          },
          success: function (res) {
            //console.log(res)
            if (res.data.result == 200) {
              var order_no = res.data.data.order_no
              wx.requestPayment({
                timeStamp: res.data.data.timeStamp,
                nonceStr: res.data.data.nonceStr,
                package: res.data.data.package,
                signType: res.data.data.signType,
                paySign: res.data.data.paySign,
                success: function (res) {
                  //console.log(res)
                  wx.request({
                    url: app.globalData.url + "payOrder",
                    data: {
                      order_no: order_no
                    },
                    method: 'POST',
                    header: {
                      'Content-Type': 'application/json'
                    },
                    success: function (res) {
                      //console.log(res)
                      wx.showModal({
                        title: '支付提示',
                        content: res.data.data,
                        showCancel: false,
                        confirmText: '我知道了'
                      })
                    },
                    fail: function (err) { app.err() },
                    complete: function () {
                      wx.hideLoading()
                    }
                  })
                },
                fail: function (res) {
                  wx.showModal({
                    title: '提示',
                    content: res,
                  })
                }
              })
              wx.hideLoading()
            }
            else {
              app.err()
            }
          },
          fail: function (err) { app.err() },
          complete: function () {
            wx.hideLoading()
          }
        })
      }
    },
    fail: function (res) {
      wx.hideLoading()
    }
  })
}