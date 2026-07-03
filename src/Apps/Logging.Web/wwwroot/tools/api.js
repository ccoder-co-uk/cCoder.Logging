window.LoggingApi = {
    token: null,

    init: function () {
        this.token = sessionStorage.getItem("loggingToken");
        this.updateAuthState();

        document.getElementById("health-check")?.addEventListener("click", () => this.health());
        document.getElementById("auth-login")?.addEventListener("click", () => this.login());
        document.getElementById("auth-logout")?.addEventListener("click", () => this.logout());
    },

    request: async function (method, url, body) {
        const headers = { Accept: "application/json" };
        const options = { method, headers };

        if (body !== undefined) {
            headers["Content-Type"] = "application/json";
            options.body = JSON.stringify(body);
        }

        if (this.token) {
            headers.Authorization = `Bearer ${this.token}`;
        }

        const response = await fetch(url, options);
        const text = await response.text();

        if (!response.ok) {
            throw new Error(text || `${response.status} ${response.statusText}`);
        }

        if (!text) {
            return null;
        }

        try {
            return JSON.parse(text);
        } catch {
            return text;
        }
    },

    get: function (url) {
        return this.request("GET", url);
    },

    post: function (url, body) {
        return this.request("POST", url, body);
    },

    unwrapCollection: function (body) {
        return Array.isArray(body?.value) ? body.value : [];
    },

    notify: function (message, isError = false) {
        const status = document.getElementById("status-message");

        if (!status) {
            return;
        }

        status.textContent = message;
        status.classList.toggle("logging-status-error", isError);
    },

    health: async function () {
        try {
            const result = await fetch("/Health");
            this.notify(await result.text());
        } catch (error) {
            this.notify(error.message, true);
        }
    },

    login: async function () {
        const username = document.getElementById("auth-username")?.value;
        const password = document.getElementById("auth-password")?.value;

        try {
            const result = await this.post("/Api/Account/Login", { user: username, pass: password });
            this.token = result?.id ?? result?.Id ?? result?.token ?? result?.Token ?? result;

            if (this.token) {
                sessionStorage.setItem("loggingToken", this.token);
            }

            this.updateAuthState(username || "Authenticated");
            this.notify("Logged in");
        } catch (error) {
            this.notify(error.message, true);
        }
    },

    logout: function () {
        this.token = null;
        sessionStorage.removeItem("loggingToken");
        this.updateAuthState();
        this.notify("Logged out");
    },

    updateAuthState: function (user) {
        const hasToken = Boolean(this.token);
        document.getElementById("auth-user").textContent = hasToken ? (user || "Authenticated") : "Guest";
        document.getElementById("auth-login").hidden = hasToken;
        document.getElementById("auth-logout").hidden = !hasToken;
        document.getElementById("auth-username").hidden = hasToken;
        document.getElementById("auth-password").hidden = hasToken;
        document.body.classList.toggle("is-authenticated", hasToken);
        document.dispatchEvent(new CustomEvent("logging-auth-changed", {
            detail: { isAuthenticated: hasToken }
        }));
    },

    isAuthenticated: function () {
        return Boolean(this.token);
    }
};

document.addEventListener("DOMContentLoaded", () => window.LoggingApi.init());
