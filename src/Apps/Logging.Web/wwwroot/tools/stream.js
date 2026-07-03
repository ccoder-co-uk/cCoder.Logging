window.LoggingStream = {
    connection: null,
    pingTimer: null,
    active: false,
    joinedThread: null,
    messages: [],
    pingIndex: 0,
    pingSamples: [
        { level: "debug", message: "Test Ping - diagnostic trace" },
        { level: "info", message: "Test Ping - stream heartbeat" },
        { level: "warning", message: "Test Ping - sample warning" },
        { level: "error", message: "Test Ping - sample error" }
    ],

    init: function () {
        document.getElementById("refresh-stream-info")
            ?.addEventListener("click", () => this.restart());

        document.addEventListener("logging-tab-changed", event => {
            if (event.detail.tabName === "stream") {
                this.start();
            } else {
                this.stopPing();
            }
        });

        document.addEventListener("logging-auth-changed", event => {
            if (!event.detail.isAuthenticated) {
                this.stop();
            }
        });

        this.renderMessages();
    },

    start: async function () {
        if (!LoggingApi.isAuthenticated()) {
            return;
        }

        this.active = true;

        if (!window.signalR) {
            this.setState("SignalR client unavailable", true);
            return;
        }

        try {
            if (!this.connection) {
                this.connection = new signalR.HubConnectionBuilder()
                    .withUrl("/Api/Hubs/Logs", {
                        accessTokenFactory: () => LoggingApi.token
                    })
                    .withAutomaticReconnect()
                    .build();

                this.connection.on("ConsoleReceive",
                    (level, message, thread) => this.receive(level, message, thread));
                this.connection.onreconnecting(() => this.setState("Reconnecting..."));
                this.connection.onreconnected(() => this.join());
                this.connection.onclose(() => this.setState("Disconnected"));

                await this.connection.start();
            }

            await this.join();
            this.startPing();
        } catch (error) {
            this.setState(error.message, true);
            LoggingApi.notify(error.message, true);
        }
    },

    stop: async function () {
        this.active = false;
        this.stopPing();

        if (!this.connection) {
            return;
        }

        try {
            await this.connection.stop();
        } finally {
            this.connection = null;
            this.joinedThread = null;
            this.setState("Disconnected");
        }
    },

    restart: async function () {
        await this.stop();

        if (document.querySelector("[data-main-tab='stream']")?.classList.contains("active")) {
            await this.start();
        }
    },

    join: async function () {
        const thread = this.thread();

        if (!thread || this.joinedThread === thread) {
            this.setState(thread ? `Connected to ${thread}` : "Thread required", !thread);
            return;
        }

        if (this.joinedThread) {
            await this.connection.invoke("Leave", this.joinedThread);
        }

        await this.connection.invoke("Join", thread);
        this.joinedThread = thread;
        this.setState(`Connected to ${thread}`);
    },

    startPing: function () {
        this.stopPing();
        this.sendPing();
        this.pingTimer = window.setInterval(() => this.sendPing(), 5000);
    },

    stopPing: function () {
        if (this.pingTimer) {
            window.clearInterval(this.pingTimer);
            this.pingTimer = null;
        }
    },

    sendPing: async function () {
        if (!this.active || !this.connection || !this.joinedThread) {
            return;
        }

        const sample = this.nextPingSample();

        try {
            await this.connection.invoke(
                "ConsoleSend",
                sample.level,
                sample.message,
                this.joinedThread);
        } catch (error) {
            this.setState(error.message, true);
        }
    },

    nextPingSample: function () {
        const sample = this.pingSamples[this.pingIndex % this.pingSamples.length];
        this.pingIndex++;

        return sample;
    },

    receive: function (level, message, thread) {
        this.messages.unshift({
            at: new Date(),
            level,
            message,
            thread
        });

        this.messages = this.messages.slice(0, 100);
        this.renderMessages();
    },

    renderMessages: function () {
        const container = document.getElementById("stream-messages");

        if (!container) {
            return;
        }

        if (this.messages.length === 0) {
            container.innerHTML = `<div class="logging-empty">No stream messages received.</div>`;
            return;
        }

        const rows = this.messages.map(message => {
            const level = String(message.level ?? "").toLowerCase();

            return `<tr class="logging-stream-row logging-stream-${this.escape(level)}">` +
            `<td class="logging-stream-time">${this.escape(message.at.toLocaleTimeString())}</td>` +
            `<td><span class="logging-stream-level">${this.escape(level)}</span></td>` +
            `<td class="logging-stream-message">${this.escape(message.message)}</td>` +
            `<td>${this.escape(message.thread)}</td>` +
            `</tr>`;
        }).join("");

        container.innerHTML =
            `<table>` +
            `<thead><tr><th>Time</th><th>Level</th><th>Message</th><th>Thread</th></tr></thead>` +
            `<tbody>${rows}</tbody>` +
            `</table>`;
    },

    setState: function (message, isError = false) {
        const state = document.getElementById("stream-state");

        if (!state) {
            return;
        }

        state.textContent = message;
        state.classList.toggle("logging-status-error", isError);
    },

    thread: function () {
        return window.location.hostname || "localhost";
    },

    escape: function (value) {
        return String(value ?? "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }
};

document.addEventListener("DOMContentLoaded", () => window.LoggingStream.init());
