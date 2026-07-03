window.LoggingGrids = {
    apiRoot: "/Api/Logging",
    initialized: false,

    configs: {
        LogEntry: {
            name: "LogEntry",
            title: "Log Entry",
            key: "Id",
            expand: "Data",
            fields: {
                Id: { label: "Id", readonly: true, create: false, type: "number" },
                AppId: { label: "App Id", type: "number", defaultValue: 1 },
                AppName: { label: "App Name", defaultValue: "localhost" },
                TypeName: { label: "Type Name", defaultValue: "Manual" },
                Level: { label: "Level", type: "number", defaultValue: 1 },
                Date: { label: "Date" },
                Message: { label: "Message", type: "textarea" }
            },
            columns: ["Id", "AppId", "AppName", "TypeName", "Level", "Date", "Message"]
        },
        LogDataItem: {
            name: "LogDataItem",
            title: "Log Data Item",
            key: "Id",
            fields: {
                Id: { label: "Id", readonly: true, create: false, type: "number" },
                LogEntryId: { label: "Log Entry Id", type: "number" },
                Name: { label: "Name" },
                Value: { label: "Value", type: "textarea" }
            },
            columns: ["Id", "LogEntryId", "Name", "Value"]
        }
    },

    store: {},

    init: function () {
        if (this.initialized || !LoggingApi.isAuthenticated()) {
            return;
        }

        this.initialized = true;
        document.getElementById("create-log-entry")
            ?.addEventListener("click", () => this.openEditor(this.configs.LogEntry, null, null));
        document.querySelectorAll("[data-main-tab]").forEach(tab =>
            tab.addEventListener("click", () => this.showMainTab(tab.dataset.mainTab)));

        this.loadLogEntries();
    },

    loadLogEntries: async function () {
        await this.loadGrid("log-entry-grid", this.configs.LogEntry, "LogEntry");
    },

    loadGrid: async function (elementId, config, scope) {
        try {
            const rows = await this.read(config);
            this.renderGrid(elementId, config, rows, scope);
            LoggingApi.notify("Ready");
        } catch (error) {
            LoggingApi.notify(error.message, true);
        }
    },

    read: async function (config) {
        let url = `${this.apiRoot}/${config.name}?$top=500&$orderby=Id desc`;

        if (config.expand) {
            url += `&$expand=${encodeURIComponent(config.expand)}`;
        }

        const body = await LoggingApi.get(url);
        return LoggingApi.unwrapCollection(body);
    },

    renderGrid: function (elementId, config, rows, scope) {
        const grid = document.getElementById(elementId);
        grid.innerHTML = this.tableHtml(config, rows, scope);
        this.bindGridActions(grid);
    },

    tableHtml: function (config, rows, scope, context = null) {
        const headers = [
            `<th class="logging-expand-column"></th>`,
            ...config.columns.map(column => `<th>${this.escape(this.label(config, column))}</th>`),
            `<th>Actions</th>`
        ].join("");
        const columnCount = config.columns.length + 2;
        const body = rows.length === 0
            ? `<tr><td colspan="${columnCount}" class="logging-empty">No ${this.escape(config.title)} rows found.</td></tr>`
            : rows.map(row => this.rowHtml(config, row, scope, context)).join("");

        return `<table class="logging-table" data-scope="${scope}">` +
            `<thead><tr>${headers}</tr></thead>` +
            `<tbody>${body}</tbody>` +
            `</table>`;
    },

    rowHtml: function (config, row, scope, context) {
        const rowKey = this.rowKey(config, row);
        const values = config.columns
            .map(column => `<td>${this.escape(this.displayValue(row[column]))}</td>`)
            .join("");
        const expandButton = config.name === "LogEntry"
            ? `<button data-action="toggle" data-scope="${scope}" data-key="${this.escape(rowKey)}" type="button">+</button>`
            : "";

        this.storeRow(scope, rowKey, row, context);

        const actions = config.name === "LogEntry"
            ? `<td class="logging-actions"><button data-action="add-data" data-scope="${scope}" data-key="${this.escape(rowKey)}" type="button">Add Data</button></td>`
            : `<td></td>`;

        return `<tr data-row-key="${this.escape(rowKey)}">` +
            `<td class="logging-expand-column">${expandButton}</td>` +
            values +
            actions +
            `</tr>`;
    },

    bindGridActions: function (container) {
        container.querySelectorAll("[data-action]").forEach(button =>
            button.addEventListener("click", event => this.onAction(event)));
    },

    storeRow: function (scope, key, row, context) {
        this.store[scope] = this.store[scope] || {};
        this.store[scope][key] = { row, context };
    },

    stored: function (scope, key) {
        return this.store[scope]?.[key] ?? null;
    },

    onAction: async function (event) {
        const button = event.currentTarget;
        const stored = this.stored(button.dataset.scope, button.dataset.key);

        if (!stored) {
            return;
        }

        if (button.dataset.action === "toggle") {
            this.toggleDetails(button, stored.row);
            return;
        }

        if (button.dataset.action === "add-data") {
            this.openEditor(this.configs.LogDataItem, null, { logEntry: stored.row });
        }
    },

    toggleDetails: function (button, logEntry) {
        const row = button.closest("tr");
        const existing = row.nextElementSibling;

        if (existing?.classList.contains("logging-detail-row")) {
            existing.remove();
            button.textContent = "+";
            return;
        }

        button.textContent = "-";
        const detailRow = document.createElement("tr");
        detailRow.className = "logging-detail-row";
        detailRow.innerHTML = `<td colspan="${row.children.length}"></td>`;
        row.after(detailRow);

        detailRow.querySelector("td").innerHTML =
            `<div class="logging-detail">` +
            `<div class="logging-detail-toolbar"><button data-add-log-data="${this.escape(logEntry.Id)}" type="button">Add Data Item</button></div>` +
            `<div data-child-grid="LogDataItem"></div>` +
            `</div>`;

        detailRow.querySelector("[data-add-log-data]")
            ?.addEventListener("click", () => this.openEditor(this.configs.LogDataItem, null, { logEntry }));

        const grid = detailRow.querySelector("[data-child-grid='LogDataItem']");
        grid.innerHTML = this.tableHtml(
            this.configs.LogDataItem,
            logEntry.Data ?? [],
            `LogDataItem-${logEntry.Id}`,
            { logEntry });
    },

    openEditor: function (config, row, context) {
        const dialog = document.getElementById("editor-dialog");
        const fields = document.getElementById("editor-fields");
        document.getElementById("editor-title").textContent = `Create ${config.title}`;
        fields.innerHTML = Object.entries(config.fields)
            .filter(([, field]) => row || field.create !== false)
            .map(([name, field]) => this.fieldHtml(name, field, row, context, config))
            .join("");

        const form = dialog.querySelector("form");
        form.onsubmit = async event => {
            event.preventDefault();

            if (event.submitter?.id === "editor-close") {
                dialog.close();
                return;
            }

            await this.saveEditor(config, context);
            dialog.close();
            await this.loadLogEntries();
        };

        dialog.showModal();
    },

    fieldHtml: function (name, field, row, context, config) {
        const contextValue = this.contextValue(name, context, config);
        const value = contextValue ?? row?.[name] ?? this.defaultValue(name, field);
        const readonly = field.readonly || contextValue !== null ? "readonly" : "";
        const input = field.type === "textarea"
            ? `<textarea name="${name}" ${readonly}>${this.escape(value)}</textarea>`
            : `<input name="${name}" value="${this.escape(value)}" ${field.type === "number" ? "type=\"number\"" : ""} ${readonly}>`;

        return `<label><span>${this.escape(field.label)}</span>${input}</label>`;
    },

    saveEditor: async function (config, context) {
        const data = this.editorPayload(config, context);
        await LoggingApi.post(`${this.apiRoot}/${config.name}`, data);
        LoggingApi.notify(`${config.title} created`);
    },

    editorPayload: function (config, context) {
        const form = document.getElementById("editor-fields");
        const payload = {};

        Object.entries(config.fields).forEach(([name, field]) => {
            const input = form.querySelector(`[name='${name}']`);

            if (!input || field.create === false && input.value === "") {
                return;
            }

            payload[name] = this.coerceInput(input, field);
        });

        if (config.name === "LogEntry" && !payload.Date) {
            payload.Date = new Date().toISOString();
        }

        if (context?.logEntry) {
            payload.LogEntryId = context.logEntry.Id;
        }

        return payload;
    },

    contextValue: function (name, context, config) {
        if (config.name === "LogDataItem" && name === "LogEntryId" && context?.logEntry) {
            return context.logEntry.Id;
        }

        return null;
    },

    showMainTab: function (tabName) {
        document.querySelectorAll("[data-main-tab]").forEach(tab =>
            tab.classList.toggle("active", tab.dataset.mainTab === tabName));
        document.querySelectorAll("[data-main-panel]").forEach(panel =>
            panel.classList.toggle("active", panel.dataset.mainPanel === tabName));
        document.dispatchEvent(new CustomEvent("logging-tab-changed", {
            detail: { tabName }
        }));
    },

    rowKey: function (config, row) {
        return row[config.key];
    },

    label: function (config, column) {
        return config.fields[column]?.label ?? column;
    },

    defaultValue: function (name, field) {
        if (field.defaultValue !== undefined) {
            return field.defaultValue;
        }

        if (field.type === "number") {
            return 0;
        }

        if (name === "Date") {
            return new Date().toISOString();
        }

        return "";
    },

    coerceInput: function (input, field) {
        if (field.type === "number") {
            return Number(input.value || 0);
        }

        return input.value;
    },

    displayValue: function (value) {
        if (value === null || value === undefined) {
            return "";
        }

        if (typeof value === "string" && value.length > 120) {
            return `${value.substring(0, 117)}...`;
        }

        return value;
    },

    escape: function (value) {
        return String(value ?? "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }
};

document.addEventListener("logging-auth-changed", event => {
    if (event.detail.isAuthenticated) {
        window.LoggingGrids.init();
    }
});

document.addEventListener("DOMContentLoaded", () => window.LoggingGrids.init());
