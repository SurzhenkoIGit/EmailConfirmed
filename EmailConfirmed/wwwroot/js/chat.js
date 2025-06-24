"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Отключаем кнопку при старте
document.getElementById("sendButton").disabled = true;

// Получение сообщения
connection.on("ReceiveMessage", function (user, message) {
    const currentUser = document.getElementById("userInput").textContent.trim();
    const isCurrentUser = user === currentUser;

    const messageItem = document.createElement("li");

    // Создаем разный дизайн для своих и чужих сообщений
    if (isCurrentUser) {
        messageItem.className = "flex justify-end mb-3";
        messageItem.innerHTML = `
            <div class="flex items-start space-x-2 max-w-[80%]">
                <div class="bg-blue-100 text-gray-800 rounded-lg py-2 px-4 break-words">
                    <p class="text-sm">${message}</p>
                    <div class="flex items-center justify-end mt-1">
                        <span class="text-xs text-gray-500">${new Date().toLocaleTimeString()}</span>
                        <i class="fas fa-check ml-1 text-blue-500"></i>
                    </div>
                </div>
                <div class="flex-shrink-0 w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center">
                    <i class="fas fa-user text-blue-500"></i>
                </div>
            </div>
        `;
    } else {
        messageItem.className = "flex justify-start mb-3";
        messageItem.innerHTML = `
            <div class="flex items-start space-x-2 max-w-[80%]">
                <div class="flex-shrink-0 w-8 h-8 rounded-full bg-gray-100 flex items-center justify-center">
                    <i class="fas fa-user text-gray-500"></i>
                </div>
                <div class="bg-gray-100 text-gray-800 rounded-lg py-2 px-4 break-words">
                    <p class="text-xs text-gray-600 mb-1">${user}</p>
                    <p class="text-sm">${message}</p>
                    <span class="text-xs text-gray-500 mt-1 block">
                        ${new Date().toLocaleTimeString()}
                    </span>
                </div>
            </div>
        `;
    }

    const messagesList = document.getElementById("messagesList");
    messagesList.appendChild(messageItem);

    // Прокрутка к последнему сообщению
    const messagesContainer = document.getElementById("messagesContainer");
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
});

// Подключение к хабу
connection.start()
    .then(function () {
        document.getElementById("sendButton").disabled = false;
        // Добавляем визуальную индикацию подключения
        const sendButton = document.getElementById("sendButton");
        sendButton.classList.remove("bg-gray-400");
        sendButton.classList.add("bg-blue-600", "hover:bg-blue-700");
    })
    .catch(function (err) {
        console.error(err.toString());
        // Визуальная индикация ошибки
        const sendButton = document.getElementById("sendButton");
        sendButton.classList.remove("bg-green-600", "hover:bg-green-700");
        sendButton.classList.add("bg-red-600");
    });

// Обработчик клика по кнопке
document.getElementById("sendButton").addEventListener("click", sendMessage);

// Обработчик нажатия Enter
document.getElementById("messageInput").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        sendMessage(event);
    }
});

// Функция отправки сообщения
function sendMessage(event) {
    const user = document.getElementById("userInput").textContent.trim();
    const messageInput = document.getElementById("messageInput");
    const message = messageInput.value.trim();

    if (message) {
        // Анимация кнопки при отправке
        const sendButton = document.getElementById("sendButton");
        sendButton.classList.add("opacity-75");

        fetch('/Chat/SendMessage', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({
                user: user,
                text: message
            })
        })
        .then(response => {
            if (!respnse.ok) {
                throw new Error("Ошибка отправки сообщения");
            }
            return response.json();
        })
        .catch(error => {
            console.error(error);
            const errorMessage = document.createElement("div");
            errorMessage.className = "text-red-500 text-sm mt-2";
            errorMessage.textContent = "Ошибка отправки сообщения";
            messageInput.parentNode.appendChild(errorMessage);
            setTimeout(() => errorMessage.remove(), 3000);
        })
        .finally(() => {
            sendButton.classList.remove("opacity-75");
            messageInput.value = "";
            messageInput.focus();
        });

        /*connection.invoke("send", user, message)
            .catch(function (err) {
                console.error(err.toString());
                // Показываем ошибку пользователю
                const errorMessage = document.createElement("div");
                errorMessage.className = "text-red-500 text-sm mt-2";
                errorMessage.textContent = "Ошибка отправки сообщения";
                messageInput.parentNode.appendChild(errorMessage);
                setTimeout(() => errorMessage.remove(), 3000);
            })
            .finally(() => {
                // Возвращаем кнопку в нормальное состояние
                sendButton.classList.remove("opacity-75");
                messageInput.value = "";
                messageInput.focus();
            });*/
    }
}

// Добавляем индикатор состояния подключения
connection.onreconnecting(() => {
    const status = document.createElement("div");
    status.className = "text-yellow-500 text-sm text-center py-2 bg-yellow-50";
    status.textContent = "Переподключение...";
    document.getElementById("messagesContainer").prepend(status);
});

connection.onreconnected(() => {
    const status = document.createElement("div");
    status.className = "text-green-500 text-sm text-center py-2 bg-green-50";
    status.textContent = "Подключено";
    document.getElementById("messagesContainer").prepend(status);
    setTimeout(() => status.remove(), 2000);
});

connection.onclose(() => {
    const status = document.createElement("div");
    status.className = "text-red-500 text-sm text-center py-2 bg-red-50";
    status.textContent = "Соединение потеряно";
    document.getElementById("messagesContainer").prepend(status);
});