const app = getApp()
var list = require('../../util/list.js')
var gid = require('../../util/gid.js')
var add = require('../../util/add.js')
var ff = "getCommentList"
var searchGid = "getShop"
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
    tab: 0
  },
  onLoad: function (options) {
    this.setData({
      searchKeyword: options.gid,
      tab: options.tab
    })
    gid.getGid(searchGid, this)
    list.getDataList(ff, this)
  },
  onShareAppMessage: function (res) {
    var that = this
    return {
      title: that.data.barTitle,
      path: '/pages/shoppage/shoppage?gid=' + that.data.searchKeyword + '&tab=' + that.data.tab
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
  buy: function (event) {
    if (app.globalData.member_gid != "") {
      if (this.data.searchGid.gid != undefined) {
        var che = {
          product_gid: this.data.searchGid.gid,
          name: this.data.searchGid.name,
          price: this.data.searchGid.price,
          number: 1,
          imgUrl: this.data.url + this.data.searchGid.picture
        }
        //存入课程购物车
        if (this.data.tab == 0) {
          var kc = [];
          kc.push(che);
          wx.setStorageSync('kc', kc)
          //课程立即购买,跳到支付页面
          wx.navigateTo({
            url: '/pages/buy/buy?tab=' + this.data.tab
          })
        }
        else//存入真柏购物车
        {
          // 获取购物车的缓存数组（没有数据，则赋予一个空数组）  
          var arr = wx.getStorageSync('cart') || [];
          // 如果购物车有数据  
          if (arr.length > 0) {
            // 遍历购物车数组  
            for (var j in arr) {
              // 判断购物车内的item的id，和事件传递过来的id，是否相等  
              if (arr[j].product_gid == this.data.searchGid.gid) {
                // 相等的话，再次添加入购物车，数量+1
                arr[j].number = arr[j].number + 1;
                // 最后，把购物车数据，存放入缓存（此处不用再给购物车数组push元素进去，因为这个是购物车有的，直接更新当前数组即可）  
                try {
                  wx.setStorageSync('cart', arr)
                  //立即购买,跳到支付页面
                  if (event.currentTarget.dataset.buy == "buy") {
                    wx.navigateTo({
                      url: '/pages/buy/buy?tab=' + this.data.tab
                    })
                  }
                  else {
                    app.ok()
                  }
                } catch (e) {
                  console.log(e)
                }
                // 返回（在if内使用return，跳出循环节约运算，节约性能）  
                return;
              }
            }
            // 遍历完购物车后，没有对应的item项，把goodslist的当前项放入购物车数组  
            arr.push(che);
          }
          // 购物车没有数据，把item项push放入当前数据（第一次存放时）  
          else {
            arr.push(che);
          }
          // 最后，把购物车数据，存放入缓存  
          try {
            wx.setStorageSync('cart', arr)
            //立即购买,跳到支付页面
            if (event.currentTarget.dataset.buy == "buy") {
              wx.navigateTo({
                url: '/pages/buy/buy?tab=' + this.data.tab
              })
            }
            else {
              app.ok()
            }
            // 返回（在if内使用return，跳出循环节约运算，节约性能）  
            return;
          } catch (e) {
            console.log(e)
          }
        }
      }
      else
      {
        wx.showToast({
          title: '商品未加载!',
          image: '/image/x.png',
          duration: 1000
        })
      }
    }
    else {
      app.loginPage()
    }
  }
})