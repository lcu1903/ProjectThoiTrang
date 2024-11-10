// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {    
    loadCart()
    var userId = $('#userAccountId').val();    
    $('.add-cart').click(function () {
        var productId = $(this).data('product-id');
        //console.log(userId);
        var quantity = 1;        
        $.ajax({
            url: '/add_cart',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                UserId: userId,
                ProductId: productId,
                Quantity: quantity
            }),
            success: function (response) {
                //console.log(response);
                if (response.value.success) {
                    $.toast({
                        heading: 'Success',
                        text: 'Thêm thành công',
                        showHideTransition: 'slide',
                        icon: 'success',
                        position: 'top-right',
                        stack: false
                    })
                    $.ajax({
                        url: '/cart_amount/' + userId, 
                        type: 'GET',
                        success: function (response) {
                            if (response.value.success) {
                                console.log(response);                                
                                $('#countCart').text(response.value.cartamout + " sản phẩm"); 
                            }
                        },
                        error: function () {
                            console.error('Failed to fetch cart amount.');
                        }
                    });
                } else {
                    $.toast({
                        heading: 'Error',
                        text: 'Quá số lượng sản phẩm hiện có',
                        showHideTransition: 'fade',
                        icon: 'error',
                        position: 'top-right',
                        stack: false
                    })
                }
            }
        });
    });    

});


function loadCart() {
    var userId = $('#userAccountId').val();    
    $.ajax({
        url: 'https://localhost:7080/cart_amount/' + userId,
        type: 'GET',
        success: function (response) {
            if (response.value.success) {                
                $('#countCart').text(response.value.cartamout + " sản phẩm");
            }
        },
        error: function () {
            console.error('Failed to fetch cart amount.');
        }
    });
}
