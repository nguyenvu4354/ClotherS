function showAlert(title, message, type = "info") {
    Swal.fire({
        title: title,
        html: message.replace(/\n/g, "<br>"), // Hiển thị danh sách theo dòng
        icon: type,
        confirmButtonText: "OK"
    });
}

$(document).ready(function () {
    $(".quantity-input").on("change", function () {
        var inputElement = $(this);
        var productId = inputElement.data("product-id");
        var quantity = parseInt(inputElement.val());

        if (quantity < 1) {
            showAlert("Error", "Quantity must be at least 1.", "error");
            inputElement.val(1);
            return;
        }

        $.ajax({
            url: "/ShoppingCart/CheckStock",
            type: "POST",
            data: { productId: productId, quantity: quantity },
            success: function (response) {
                if (!response.success) {
                    showAlert("Stock Alert", "Only " + response.maxQuantity + " items available in stock.", "warning");
                    inputElement.val(response.maxQuantity);
                }
                updateCart(productId, inputElement.val());
            }
        });
    });

    function updateCart(productId, quantity) {
        $.ajax({
            url: "/ShoppingCart/UpdateCart",
            type: "POST",
            data: { productId: productId, quantity: quantity },
            success: function () {
                console.log("Cart updated successfully.");
                updateTotalPrice(productId, quantity);
            },
            error: function () {
                showAlert("Error", "Failed to update cart.", "error");
            }
        });
    }

    function updateTotalPrice(productId, quantity) {
        var row = $(`.quantity-input[data-product-id='${productId}']`).closest("tr");
        var price = parseFloat(row.find("td:nth-child(3)").text().replace("$", ""));
        var discount = parseFloat(row.find("td:nth-child(4)").text().replace("$", "")) || 0;
        var total = (quantity * (price - discount)).toFixed(2);
        row.find("td:nth-child(6)").text(`$${total}`);
    }

    $("#cartForm").on("submit", function (event) {
        let selectedProducts = $(".product-checkbox:checked");
        if (selectedProducts.length === 0) {
            showAlert("Error", "Please select at least one product to purchase.", "error");
            event.preventDefault();
            return;
        }

        let unavailableProducts = [];
        selectedProducts.each(function () {
            let row = $(this).closest("tr");
            let productName = row.find("td:nth-child(2)").text().trim();
            if ($(this).data("disable") === true) {
                unavailableProducts.push(productName);
            }
        });

        if (unavailableProducts.length > 0) {
            let message = unavailableProducts.join("\n") + "is no longer available:\n" ;
            showAlert("Unavailable Products", message, "warning");
            event.preventDefault();
        }
    });
});
