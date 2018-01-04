const app = getApp()
Page({
  data: {
    contact_number: '',
    address: '',
    real_name: '',
    tab: 0,
    iscart: false,
    cart: [], //数据  
    total: 0,    //总金额  
    goodsCount: 0 //数量  
  },
  onLoad: function (options) {
    this.setData({
      tab: options.tab
    })
    getAddr(this)
  },
  onShow: function () {
    var arr
    if (this.data.tab == 0) {
      // 获取产品展示页保存的缓存数据（购物车的缓存数组，没有数据，则赋予一个空数组）  
      arr = wx.getStorageSync('kc') || []
    }
    else {
      arr = wx.getStorageSync('cart') || []
    }
    // 有数据的话，就遍历数据，计算总金额 和 总数量  
    if (arr.length > 0) {
      for (var i in arr) {
        this.data.total += Number(arr[i].price * arr[i].number)
        this.data.goodsCount += Number(arr[i].number)
      }
      // 更新数据  
      this.setData({
        iscart: true,
        cart: arr,
        total: this.data.total,
        goodsCount: this.data.goodsCount
      })
    }
    //获取收货地址
  },
  /* 减数 */
  delCount: function (e) {
    var index = [e.target.id.substring(3)]
    // 获取购物车该商品的数量  
    // [获取设置在该btn的id,即list的index值]  
    if (this.data.cart[index].number <= 1) {
      return;
    }
    // 商品总数量-1  
    this.data.goodsCount -= 1;
    // 总价钱 减去 对应项的价钱单价  
    this.data.total -= Number(this.data.cart[index].price);
    // 购物车主体数据对应的项的数量-1  并赋给主体数据对应的项内  
    this.data.cart[index].number = --this.data.cart[index].number;
    // 更新data数据对象  
    this.setData({
      cart: this.data.cart,
      total: this.data.total,
      goodsCount: this.data.goodsCount
    })
    // 主体数据重新赋入缓存内  
    try {
      wx.setStorageSync('cart', this.data.cart)
    } catch (e) {
      console.log(e)
    }
  },
  /* 加数 */
  addCount: function (e) {
    var index = [e.target.id.substring(3)]
    // 商品总数量+1  
    this.data.goodsCount += 1;
    // 总价钱 加上 对应项的价钱单价  
    this.data.total += Number(this.data.cart[index].price);
    // 购物车主体数据对应的项的数量+1  并赋给主体数据对应的项内  
    this.data.cart[index].number = ++this.data.cart[index].number;
    // 更新data数据对象  
    this.setData({
      cart: this.data.cart,
      total: this.data.total,
      goodsCount: this.data.goodsCount
    })
    // 主体数据重新赋入缓存内  
    try {
      wx.setStorageSync('cart', this.data.cart)
    } catch (e) {
      console.log(e)
    }
  },
  /* 删除item */
  delGoods: function (e) {
    var index = [e.target.id.substring(3)]
    // 商品总数量  减去  对应删除项的数量  
    this.data.goodsCount = this.data.goodsCount - this.data.cart[index].number;
    // 总价钱  减去  对应删除项的单价*数量  
    this.data.total -= this.data.cart[index].price * this.data.cart[index].number;
    // 主体数据的数组移除该项  
    this.data.cart.splice(e.target.id.substring(3), 1);
    // 更新data数据对象  
    this.setData({
      cart: this.data.cart,
      total: this.data.total,
      goodsCount: this.data.goodsCount,
      iscart: this.data.goodsCount < 1 ? false : true
    })
    // 主体数据重新赋入缓存内  
    try {
      wx.setStorageSync('cart', this.data.cart)
    } catch (e) {
      console.log(e)
    }
  },
  formBindsubmit: function (e) {
    var that = this
    var contact_number = e.detail.value.contact_number
    var consignee = e.detail.value.consignee
    var address = e.detail.value.address
    var remarks = e.detail.value.remarks
    if (contact_number.length < 10 || consignee.length < 2 || address.length < 5) {
      wx.showToast({
        title: '信息不完整',
        image: '/image/x.png',
        duration: 2000
      })
    } else {
      if (app.globalData.member_gid != "") {
        if (that.data.total > 0) {
          app.showLoading()
          wx.request({
            url: app.globalData.url + "order",
            data: {
              searchKeyword: "",
              member_gid: app.globalData.member_gid,
              login_identifier: app.globalData.login_identifier,
              product: that.data.cart,
              type: that.data.tab == 0 ? 3 : 1,
              contact_number: contact_number,
              consignee: consignee,
              address: address,
              remarks: remarks
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
                        var rqres = res
                        wx.showModal({
                          title: '支付提示',
                          content: res.data.data,
                          showCancel: false,
                          confirmText: '我知道了',
                          success: function (res) {
                            if (rqres.data.result == 200) {
                              // 更新data数据对象  
                              that.setData({
                                cart: [],
                                total: 0,
                                goodsCount: 0,
                                iscart: false
                              })
                              // 主体数据重新赋入缓存内  
                              try {
                                wx.setStorageSync('cart', that.data.cart)
                              } catch (e) {
                                console.log(e)
                              }
                              if (that.data.tab == 0) {
                                wx.redirectTo({
                                  url: '/pages/kecheng/kecheng'
                                })
                              }
                              else {
                                wx.redirectTo({
                                  url: '/pages/zhenbo/zhenbo'
                                })
                              }
                            }
                          }
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
                      content: '支付失败',
                      showCancel: false,
                      confirmText: '我知道了'
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
        else {
          wx.showToast({
            title: '金额少于0',
            image: '/image/x.png',
            duration: 2000
          })
        }
      }
      else {
        app.loginPage()
      }
    }
  }
})

function getAddr(that) {
  wx.request({
    url: app.globalData.url + "getAddr",
    data: {
      member_gid: app.globalData.member_gid
    },
    method: 'POST',
    header: {
      'Content-Type': 'application/x-www-form-urlencoded'
    },
    success: function (res) {
      if (res.data.result == 200) {
        that.setData({
          contact_number: res.data.data.contact_number,
          address: res.data.data.address,
          real_name: res.data.data.real_name
        })
      }
    }
  })
}