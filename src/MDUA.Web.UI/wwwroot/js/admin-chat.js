/* =========================================================
   FILENAME: wwwroot/js/admin-chat.js
   ========================================================= */

var adminChat = (function () {
    var _hubConnection = null;

    function init() {
        console.log("Initializing Admin Chat...");

        _hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/supportHub")
            .withAutomaticReconnect()
            .build();

        _hubConnection.start()
            .then(() => console.log("✅ Admin Chat SignalR Connected"))
            .catch(err => console.error(err));

        // EVENT: Receive Message (Real-time updates in Admin Panel)
        _hubConnection.on("ReceiveMessage", function (user, message, sessionGuid) {
            loadHeaderChats();
            if ($('#admin-chat-panel').is(':visible')) {
                loadActiveChats();
            }
            var currentGuid = $('#current-chat-guid').val();
            if (currentGuid && currentGuid.toLowerCase() === sessionGuid.toLowerCase()) {
                appendBubble(user, message, false);
            }
        });

        loadHeaderChats();
        bindEvents();
    }

    function bindEvents() {
        $('#header-msg-btn').off('click').on('click', function (e) {
            e.stopPropagation();
            $('#header-msg-dropdown').toggle();
            if ($('#header-msg-dropdown').is(':visible')) loadHeaderChats();
        });

        $(document).on('click', function () { $('#header-msg-dropdown').hide(); });
        $('#header-msg-dropdown').on('click', function (e) { e.stopPropagation(); });

        $('#admin-send-btn').off('click').on('click', sendReply);
        $('#admin-chat-input').off('keypress').on('keypress', function (e) {
            if (e.which == 13) sendReply();
        });
    }

    function loadHeaderChats() {
        $.get('/chat/active-sessions', function (data) {
            const list = $('#header-chat-list');
            list.empty();
            let totalUnread = 0;

            if (!data || data.length === 0) {
                list.append('<div class="text-center p-4 text-muted small">No active conversations.</div>');
                $('#header-msg-badge').hide();
                return;
            }

            data.forEach(function (session) {
                totalUnread += (session.unreadCount || 0);
                let bgStyle = session.unreadCount > 0 ? "background-color: #f0f9ff;" : "background-color: #fff;";
                let nameWeight = session.unreadCount > 0 ? "font-weight: 800; color: #000;" : "font-weight: 600; color: #334155;";
                let badgeHtml = session.unreadCount > 0 ? `<span class="badge bg-primary rounded-pill ms-2">${session.unreadCount}</span>` : '';
                let statusText = session.unreadCount > 0 ? '<span class="text-primary fw-bold">New messages</span>' : '<span class="text-muted">Click to view</span>';
                let initials = (session.guestName || "Gu").substring(0, 2).toUpperCase();
                let time = new Date(session.lastMessageAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

                let html = `
                <div onclick="adminChat.openFromHeader('${session.id}', '${session.sessionGuid}', '${session.guestName}')"
                     style="padding: 12px 15px; border-bottom: 1px solid #f1f5f9; cursor: pointer; display: flex; align-items: center; gap: 12px; transition:0.2s; ${bgStyle}">
                    <div style="width: 40px; height: 40px; background: #e2e8f0; color: #475569; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: bold;">${initials}</div>
                    <div style="flex:1; min-width:0;">
                        <div class="d-flex justify-content-between align-items-center mb-1">
                            <span class="text-truncate" style="font-size:0.9rem; ${nameWeight}">${session.guestName} ${badgeHtml}</span>
                            <span class="text-muted" style="font-size:0.7rem;">${time}</span>
                        </div>
                        <div class="text-truncate small" style="font-size:0.75rem;">${statusText}</div>
                    </div>
                </div>`;
                list.append(html);
            });

            const badge = $('#header-msg-badge');
            if (totalUnread > 0) {
                badge.text(totalUnread).show();
                $('#header-msg-btn i').removeClass('text-secondary').addClass('text-primary');
            } else {
                badge.hide();
                $('#header-msg-btn i').removeClass('text-primary').addClass('text-secondary');
            }
        });
    }

    function loadActiveChats() {
        $.get('/chat/active-sessions', function (data) {
            $('#admin-chat-list').empty();
            if (!data || data.length === 0) {
                $('#admin-chat-list').append('<div style="padding:15px; color:#666; text-align:center;">No active chats</div>');
                return;
            }
            data.forEach(function (session) {
                let time = new Date(session.lastMessageAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
                let isActive = session.id == $('#current-chat-session-id').val() ? 'background-color: #e0f2fe;' : '';
                let unreadDot = session.unreadCount > 0 ? '<span style="width:8px; height:8px; background:red; border-radius:50%; display:inline-block; margin-right:5px;"></span>' : '';

                let html = `
                    <div onclick="adminChat.openSession('${session.id}', '${session.sessionGuid}', '${session.guestName}')"
                         style="padding: 15px; border-bottom: 1px solid #f1f5f9; cursor: pointer; transition: background 0.2s; ${isActive}"
                         onmouseover="this.style.background='#f1f5f9'" onmouseout="this.style.background='${isActive ? '#e0f2fe' : 'transparent'}'">
                        <div style="font-weight: 600; color: #334155; margin-bottom: 4px;">${unreadDot} ${session.guestName}</div>
                        <div style="font-size: 0.8rem; color: #64748b; display: flex; justify-content: space-between;">
                            <span>${session.status}</span><span>${time}</span>
                        </div>
                    </div>`;
                $('#admin-chat-list').append(html);
            });
        });
    }

    // 🌟 RESTORED: Logic to join with specific name
    function openSession(id, guid, name) {
        $('#current-chat-session-id').val(id);
        $('#current-chat-guid').val(guid);
        $('#chat-current-guest').text(name);
        $('#admin-chat-history').html('<div style="text-align:center; padding:20px;">Loading history...</div>');

        // Refresh lists
        setTimeout(function () { loadHeaderChats(); if ($('#admin-chat-panel').is(':visible')) loadActiveChats(); }, 1000);

        // ✅ RESTORED: Create the specific name string
        var adminName = $('#current-admin-name').val() || "Admin";
        var joinMessage = "Support Agent - " + adminName;

        // Send to Hub
        if (_hubConnection && _hubConnection.state === signalR.HubConnectionState.Connected) {
            _hubConnection.invoke("AdminJoinSession", joinMessage, guid);
        }

        $.get('/chat/history?sessionId=' + id, function (messages) {
            $('#admin-chat-history').empty();
            messages.forEach(m => appendBubble(m.senderName, m.messageText, m.isFromAdmin));
        });
    }
    function openFromHeader(id, guid, name) {
        $('#header-msg-dropdown').hide();
        $('#admin-chat-panel').fadeIn();
        openSession(id, guid, name);
    }

    // 🌟 RESTORED: Logic to display "Sent by Name"
    function sendReply() {
        const msg = $('#admin-chat-input').val().trim();
        const guid = $('#current-chat-guid').val();

        // Get Local Name for UI
        var adminName = $('#current-admin-name').val() || "Admin";

        if (!msg || !guid) return;

        // Pass 'adminName' so the hub can use it OR hub uses server-side identity
        // But for local bubble, we definitely use adminName
        _hubConnection.invoke("SendReplyToUser", adminName, msg, guid)
            .then(function () {
                appendBubble(adminName, msg, true);
                $('#admin-chat-input').val('');
            });
    }

    function appendBubble(user, text, isSelf) {
        let align = isSelf ? 'align-self: flex-end;' : 'align-self: flex-start;';
        let bg = isSelf ? 'background: #2563eb; color: white;' : 'background: #f1f5f9; color: #1e293b;';
        let radius = isSelf ? 'border-radius: 12px 12px 0 12px;' : 'border-radius: 12px 12px 12px 0;';

        // Logic: If me, show "Sent by Name", else just Name
        let label = isSelf ? `Sent by ${user}` : user;

        let html = `<div style="${align} max-width: 70%; ${bg} padding: 10px 14px; ${radius} font-size: 0.95rem; line-height: 1.5; margin-bottom: 5px;">${text}</div>
                    <div style="${align} font-size: 0.7rem; color: #94a3b8; margin-bottom: 8px;">${label}</div>`;

        let history = $('#admin-chat-history');
        if (history.length > 0) {
            history.append(html);
            history.scrollTop(history[0].scrollHeight);
        }
    }

    function togglePanel() {
        $('#admin-chat-panel').toggle();
        if ($('#admin-chat-panel').is(':visible')) loadActiveChats();
    }

    return {
        init: init,
        openSession: openSession,
        openFromHeader: openFromHeader,
        refreshActive: loadActiveChats,
        togglePanel: togglePanel
    };
})();

$(document).ready(function () {
    adminChat.init();
});