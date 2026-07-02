// SmartBank.Web - small client-side UX helpers.
// Bootstrap unobtrusive validation is wired automatically via jquery.validate.unobtrusive
// against the [Required]/[Range]/[Compare]/etc. data annotations on each PageModel's
// [BindProperty], so most forms need nothing extra here.

(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {
        // Auto-dismiss success/error alerts after a few seconds.
        var alerts = document.querySelectorAll(".alert.alert-success, .alert.alert-danger");
        alerts.forEach(function (el) {
            setTimeout(function () {
                var bsAlert = bootstrap.Alert.getOrCreateInstance(el);
                if (bsAlert) {
                    bsAlert.close();
                }
            }, 6000);
        });

        // Confirm destructive actions (delete customer / close account) client-side
        // before the (server re-validated) POST goes out.
        document.querySelectorAll("[data-confirm]").forEach(function (el) {
            el.addEventListener("submit", function (e) {
                var message = el.getAttribute("data-confirm") || "Are you sure?";
                if (!window.confirm(message)) {
                    e.preventDefault();
                }
            });
        });

        // Simple currency formatting hint on amount inputs (visual only; server validates).
        document.querySelectorAll("input[data-currency]").forEach(function (input) {
            input.addEventListener("blur", function () {
                var val = parseFloat(input.value);
                if (!isNaN(val)) {
                    input.value = val.toFixed(2);
                }
            });
        });
    });
})();
