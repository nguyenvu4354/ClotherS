function toggleSidebar() {
    document.getElementById("adminSidebar").classList.toggle("hidden");
}

// Ẩn sidebar khi click ra ngoài
document.addEventListener("click", function (event) {
    var sidebar = document.getElementById("adminSidebar");
    var button = document.querySelector(".toggle-btn");

    if (!sidebar.contains(event.target) && !button.contains(event.target)) {
        sidebar.classList.add("hidden");
    }
});
