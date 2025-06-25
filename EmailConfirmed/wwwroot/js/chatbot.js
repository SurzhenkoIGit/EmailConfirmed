/*let currentPath = [];

document.addEventListener('DOMContentLoaded', function() {
    const chatButton = document.getElementById('chatButton');
    const icon = document.getElementById('icon');
    const chatPopup = document.getElementById('chatPopup');

    // Устанавливаем начальное состояние
    chatPopup.style.display = "none";

    chatButton.onclick = function() {
        icon.classList.add('hiding');

        setTimeout(() => {
            if (chatPopup.style.display === "none") {
                // Показываем чат
                chatPopup.style.display = "block";
                chatPopup.classList.remove('hide');
                chatPopup.classList.add('show');
                
                icon.classList.remove("fa-robot");
                icon.classList.add("fa-times");
                
                // Загружаем начальные темы
                handleTopic(null);
            } else {
                // Скрываем чат
                chatPopup.classList.remove('show');
                chatPopup.classList.add('hide');
                
                icon.classList.remove("fa-times");
                icon.classList.add("fa-robot");

                // Ждем окончания анимации перед скрытием
                setTimeout(() => {
                    chatPopup.style.display = "none";
                }, 300);
            }

            requestAnimationFrame(() => {
                icon.classList.remove('hiding');
            });
        }, 150);
    };

    // Добавляем обработчик окончания анимации
    chatPopup.addEventListener('animationend', function(e) {
        if (e.animationName === 'popOut') {
            chatPopup.style.display = 'none';
        }
    });

    // ... остальной код без изменений ...
});

async function handleTopic(topicId, newPath = []) {
    currentPath = newPath;
    try {
        const response = await fetch(`/Chatbot/GetTopics?${currentPath.map((p, i) => `path[${i}]=${p}`).join('&')}`);
        const topics = await response.json();
        
        const messagesDiv = document.getElementById("messages");
        messagesDiv.innerHTML = '';
        
        // Создаем контейнер для всех кнопок
        const topicsContainer = document.createElement('div');
        topicsContainer.className = 'topics-container';
        
        // Добавляем кнопку "Назад", если мы не на главном уровне
        if (currentPath.length > 0) {
            const backButton = document.createElement('button');
            backButton.className = 'back-button';
            backButton.innerHTML = '<i class="fas fa-arrow-left"></i> Назад';
            backButton.onclick = () => {
                currentPath.pop();
                handleTopic(null, currentPath);
            };
            topicsContainer.appendChild(backButton);
        }
        
        // Добавляем кнопки тем
        topics.forEach(topic => {
            const button = document.createElement("button");
            button.className = "topic-button";
            if (topic.hasSubtopics) {
                button.className += " has-subtopics";
            }
            
            // Создаем иконку для темы
            const icon = document.createElement('i');
            icon.className = topic.hasSubtopics ? 'fas fa-folder' : 'fas fa-comment';
            
            // Создаем текст кнопки
            const text = document.createTextNode(topic.title);
            
            // Добавляем иконку и текст в кнопку
            button.appendChild(icon);
            button.appendChild(text);
            
            if (topic.hasSubtopics) {
                button.onclick = () => handleTopic(topic.id, [...currentPath, topic.id]);
            } else {
                button.onclick = () => showResponse([...currentPath, topic.id]);
            }
            
            topicsContainer.appendChild(button);
        });
        
        messagesDiv.appendChild(topicsContainer);
    } catch (error) {
        console.error('Ошибка при получении тем:', error);
    }
}

async function showResponse(path) {
    try {
        const response = await fetch(`/Chatbot/GetResponse?${path.map((p, i) => `path[${i}]=${p}`).join('&')}`);
        const data = await response.json();
        
        const messagesDiv = document.getElementById("messages");
        const messageDiv = document.createElement('div');
        messageDiv.className = 'message bot';
        messageDiv.textContent = data.response;
        messagesDiv.appendChild(messageDiv);
        
        // Добавляем кнопку для возврата к темам
        const backButton = document.createElement('button');
        backButton.className = 'topic-button';
        backButton.textContent = 'Вернуться к темам';
        backButton.onclick = () => handleTopic(null, currentPath);
        messagesDiv.appendChild(backButton);
    } catch (error) {
        console.error('Ошибка при получении ответа:', error);
    }
}

// Обновляем функцию Send для работы с новой структурой
function Send(event) {
    const userInput = document.getElementById("userInput").value;
    if (userInput.trim() !== "") {
        const messageDiv = document.createElement('div');
        messageDiv.className = 'message user';
        messageDiv.textContent = userInput;
        document.getElementById("messages").appendChild(messageDiv);
        document.getElementById("userInput").value = "";
        
        // Возвращаемся к главному меню тем
        handleTopic(null);
    }
}

async function sendMessage(event) {
    const userInput = document.getElementById("userInput");
    const message = userInput.value.trim();
    
    if (message !== "") {
        // Добавляем сообщение пользователя
        appendMessage(message, 'user');
        userInput.value = "";

        try {
            // Отправляем запрос к серверу
            const response = await fetch('/Chatbot/SendMessage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ message: message })
            });

            const result = await response.json();
            
            // Добавляем ответ бота с небольшой задержкой
            setTimeout(() => {
                appendMessage(result.response, 'bot');
            }, 500);

        } catch (error) {
            console.error('Ошибка:', error);
            appendMessage('Извините, произошла ошибка. Попробуйте позже.', 'bot error');
        }
    }
}

function appendMessage(message, type) {
    const messagesDiv = document.getElementById("messages");
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${type}`;
    
    // Создаем контейнер для аватара и текста
    const containerDiv = document.createElement('div');
    containerDiv.className = 'message-container';
    
    // Добавляем аватар
    const avatarDiv = document.createElement('div');
    avatarDiv.className = 'avatar';
    avatarDiv.innerHTML = type === 'user' ? 
        '<i class="fas fa-user"></i>' : 
        '<i class="fas fa-robot"></i>';
    
    // Добавляем текст сообщения
    const textDiv = document.createElement('div');
    textDiv.className = 'message-text';
    textDiv.textContent = message;
    
    // Собираем все вместе
    containerDiv.appendChild(avatarDiv);
    containerDiv.appendChild(textDiv);
    messageDiv.appendChild(containerDiv);
    messagesDiv.appendChild(messageDiv);
    
    // Прокручиваем к последнему сообщению
    messagesDiv.scrollTop = messagesDiv.scrollHeight;
}

// Обновляем обработчики событий в DOMContentLoaded
document.addEventListener('DOMContentLoaded', function() {
    // ... существующие обработчики ...

    // Обновляем обработчики отправки сообщений
    document.getElementById("sendMessage").addEventListener("click", sendMessage);
    
    document.getElementById("userInput").addEventListener("keypress", function(event) {
        if (event.key === "Enter") {
            event.preventDefault();
            sendMessage(event);
        }
    });
});*/
document.addEventListener('DOMContentLoaded', function () {
    const chatButton = document.getElementById('chatButton');
    const chatWindow = document.getElementById('chatWindow');
    const closeChat = document.getElementById('closeChat');
    const messageInput = document.getElementById('messageInput');
    const sendMessage = document.getElementById('sendMessage');
    const chatMessages = document.getElementById('chatMessages');
    const topicButtons = document.getElementById('topicButtons');
    const robotIcon = chatButton.querySelector('.fa-robot');
    const closeIcon = chatButton.querySelector('.fa-times');

    let currentTopics = [];

    // Загрузка тем при инициализации
    async function loadTopics() {
        try {
            const response = await fetch('/Chatbot/GetTopics');
            if (!response.ok) throw new Error('Failed to load topics');

            const topics = await response.json();
            currentTopics = topics;
            renderTopicButtons(topics);
        } catch (error) {
            console.error('Error loading topics:', error);
        }
    }

    // Отрисовка кнопок тем
    function renderTopicButtons(topics) {
        topicButtons.innerHTML = ''; // Очищаем существующие кнопки

        topics.forEach(topic => {
            const button = document.createElement('button');
            button.className = getTopicButtonClass(topic.type);
            button.textContent = topic.name;
            button.addEventListener('click', () => handleTopicClick(topic));
            topicButtons.appendChild(button);
        });
    }

    // Обработка клика по теме
    function handleTopicClick(topic) {
        if (topic.subtopics && topic.subtopics.length > 0) {
            // Если есть подтемы, показываем их
            renderTopicButtons(topic.subtopics);

            // Добавляем кнопку "Назад"
            const backButton = document.createElement('button');
            backButton.className = 'bg-gray-100 hover:bg-gray-200 text-gray-700 px-3 py-1 rounded-full text-sm transition-colors duration-200';
            backButton.textContent = '← Назад';
            backButton.addEventListener('click', () => renderTopicButtons(currentTopics));
            topicButtons.insertBefore(backButton, topicButtons.firstChild);
        } else {
            // Если это конечная тема, отправляем сообщение
            messageInput.value = topic.message || `Расскажите про ${topic.name.toLowerCase()}`;
            sendMessageHandler();
        }
    }

    // Получение класса для кнопки темы
    function getTopicButtonClass(type) {
        const baseClass = 'px-3 py-1 rounded-full text-sm transition-colors duration-200';
        switch (type) {
            case 'general':
                return `bg-blue-100 hover:bg-blue-200 text-blue-700 ${baseClass}`;
            case 'weather':
                return `bg-green-100 hover:bg-green-200 text-green-700 ${baseClass}`;
            case 'news':
                return `bg-purple-100 hover:bg-purple-200 text-purple-700 ${baseClass}`;
            case 'map':
                return `bg-yellow-100 hover:bg-yellow-200 text-yellow-700 ${baseClass}`;
            case 'help':
                return `bg-red-100 hover:bg-red-200 text-red-700 ${baseClass}`;
            default:
                return `bg-gray-100 hover:bg-gray-200 text-gray-700 ${baseClass}`;
        }
    }

    // Показать/скрыть окно чата
    chatButton.addEventListener('click', () => {
        const isHidden = chatWindow.classList.contains('hidden');

        // Анимация иконки
        if (isHidden) {
            // Открываем чат
            robotIcon.classList.add('scale-0', 'rotate-180');
            closeIcon.classList.remove('scale-0');
            closeIcon.classList.add('rotate-180');

            // Показываем окно
            chatWindow.classList.remove('hidden');
            // Даем время для инициализации transition
            setTimeout(() => {
                chatWindow.classList.remove('opacity-0', 'translate-y-4');
            }, 50);

            messageInput.focus();
        } else {
            // Закрываем чат
            robotIcon.classList.remove('scale-0', 'rotate-180');
            closeIcon.classList.add('scale-0');
            closeIcon.classList.remove('rotate-180');

            // Скрываем окно
            chatWindow.classList.add('opacity-0', 'translate-y-4');
            // Ждем окончания анимации перед скрытием
            setTimeout(() => {
                chatWindow.classList.add('hidden');
            }, 300);
        }
    });

    closeChat.addEventListener("click", () => {
        robotIcon.classList.remove('scale-0', 'rotate-180');
        closeIcon.classList.add('scale-0');
        closeIcon.classList.remove('rotate-180');

        // Скрываем окно
        chatWindow.classList.add('opacity-0', 'translate-y-4');
        // Ждем окончания анимации перед скрытием
        setTimeout(() => {
            chatWindow.classList.add('hidden');
        }, 300);
    })

    // Добавим анимацию при первой загрузке
    chatButton.classList.add('animate-bounce');
    setTimeout(() => {
        chatButton.classList.remove('animate-bounce');
    }, 2000);

    // Анимация при наведении на кнопку
    chatButton.addEventListener('mouseenter', () => {
        if (chatWindow.classList.contains('hidden')) {
            robotIcon.classList.add('animate-wiggle');
        }
    });

    chatButton.addEventListener('mouseleave', () => {
        robotIcon.classList.remove('animate-wiggle');
    });

    // Отправка сообщения
    async function sendMessageHandler() {
        const message = messageInput.value.trim();
        if (message) {
            addMessage('user', message, true);
            messageInput.value = '';
            messageInput.focus();

            showLlamaGenerating();

            try {
                const response = await fetch('/Chatbot/SendMessage', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ message: message })
                });

                const result = await response.json();
                removeLlamaGenerating();

                addMessage('bot', result.response, true);
            } catch (error) {
                console.error('Error:', error);
                removeLlamaGenerating();
                addMessage('error', 'Извините, произошла ошибка. Попробуйте позже.');
            }
        }
    }

    // Обработка отправки
    sendMessage.addEventListener('click', sendMessageHandler);
    messageInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessageHandler();
        }
    });

    function showLlamaGenerating() {
        const generatingDiv = document.createElement('div');
        generatingDiv.id = 'llamaGenerating';
        generatingDiv.className = 'flex justify-start mb-4';
        generatingDiv.innerHTML = `
            <div class="bg-gray-100 rounded-lg p-3 shadow-sm">
                <div class="flex space-x-2">
                    <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
                    <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
                    <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.4s"></div>
                </div>
            </div>
        `;
        chatMessages.appendChild(generatingDiv);
        scrollToBottom();
    }

    function removeLlamaGenerating() {
        const generatingIndicator = document.getElementById('llamaGenerating');
        if (generatingIndicator) {
            generatingIndicator.remove();
        }
    }

    // Добавление сообщения в чат
    function addMessage(type, text, isLlama) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `flex ${type === 'user' ? 'justify-end' : 'justify-start'} mb-4`;

        const messageBubble = document.createElement('div');
        messageBubble.className = getMessageClass(type);

        // Форматируем текст
        const formattedText = isLlama ? formatLlamaResponse(text) : formatMessage(text);

        messageBubble.innerHTML = formattedText;
        messageDiv.appendChild(messageBubble);
        chatMessages.appendChild(messageDiv);
        scrollToBottom();
    }

    function formatLlamaResponse(text) {
        text = text.replace(/\n/g, '<br>');

        text = text.replace(/```([^`]+)```/g, '<pre><code>$1</code></pre>');
        text = text.replace(/`([^`]+)`/g, '<code>$1</code>');

        text = text.replace(/^\s*[-*]\s+(.+)$/gm, '<li>$1</li>');

        return `<div class="llama-response">${text}</div>`;
    }

    // Получение класса для сообщения в зависимости от типа
    function getMessageClass(type) {
        const baseClasses = 'max-w-[75%] rounded-lg p-3 shadow-sm';
        switch (type) {
            case 'user':
                return `${baseClasses} bg-blue-500 text-white ml-auto`;
            case 'bot':
                return `${baseClasses} bg-gray-100 text-gray-800`;
            case 'error':
                return `${baseClasses} bg-red-100 text-red-700`;
            default:
                return `${baseClasses} bg-gray-100 text-gray-800`;
        }
    }

    // Форматирование текста сообщения
    function formatMessage(text) {
        // Заменяем URL на кликабельные ссылки
        text = text.replace(
            /(https?:\/\/[^\s]+)/g,
            '<a href="$1" target="_blank" class="underline hover:text-blue-500">$1</a>'
        );

        // Заменяем переносы строк на <br>
        text = text.replace(/\n/g, '<br>');

        // Выделяем код
        text = text.replace(
            /`([^`]+)`/g,
            '<code class="bg-gray-200 px-1 rounded text-sm font-mono">$1</code>'
        );

        return text;
    }

    // Индикатор набора текста
    function showTypingIndicator() {
        const typingDiv = document.createElement('div');
        typingDiv.id = 'typingIndicator';
        typingDiv.className = 'flex justify-start mb-4';
        typingDiv.innerHTML = `
            <div class="bg-gray-100 rounded-lg p-3 shadow-sm">
                <div class="flex space-x-2">
                    <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
                    <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
                    <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.4s"></div>
                </div>
            </div>
        `;
        chatMessages.appendChild(typingDiv);
        scrollToBottom();
    }

    function removeTypingIndicator() {
        const typingIndicator = document.getElementById('typingIndicator');
        if (typingIndicator) {
            typingIndicator.remove();
        }
    }

    // Прокрутка чата вниз
    function scrollToBottom() {
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    function initializeChat() {
        loadTopics(); // Загружаем темы
        addMessage('bot', 'Здравствуйте! Я ваш виртуальный помощник. Выберите тему или задайте вопрос.');
    }

    // Вызываем инициализацию чата
    initializeChat();
});