document.addEventListener("DOMContentLoaded", function () {
  const chatbotContainer = document.getElementById("chatbot-container");
  const closeBtn = document.getElementById("close-btn");
  const sendBtn = document.getElementById("send-btn");
  const chatBotInput = document.getElementById("chatbot-input");
  const chatbotMessages = document.getElementById("chatbot-messages");
  const chatbotIcon = document.getElementById("chatbot-icon");

  // Suggested questions
  const suggestedQuestions = [
    "Tôi muốn mua áo?",
    "Tôi muốn mua quần dài?",
    "Có những danh mục sản phẩm nào?",
    "Sản phẩm mới nhất là gì?",
    "Có chương trình khuyến mãi nào không?"
  ];

  chatbotIcon.addEventListener("click", () => {
    chatbotContainer.classList.remove("hidden");
    chatbotIcon.style.display = "none";
    // Add welcome message and suggested questions
    appendMessage("bot", "Xin chào! Tôi có thể giúp gì cho bạn?");
    showSuggestedQuestions();
  });

  closeBtn.addEventListener("click", () => {
    chatbotContainer.classList.add("hidden");
    chatbotIcon.style.display = "flex";
    // Clear messages when closing
    chatbotMessages.innerHTML = "";
  });

  sendBtn.addEventListener("click", sendMessage);

  chatBotInput.addEventListener("keypress", (e) => {
    if (e.key === "Enter") sendMessage();
  });

  // Function to show suggested questions
  function showSuggestedQuestions() {
    const suggestionsContainer = document.createElement("div");
    suggestionsContainer.className = "suggestions-container";
    
    suggestedQuestions.forEach(question => {
      const suggestionBtn = document.createElement("button");
      suggestionBtn.className = "suggestion-btn";
      suggestionBtn.textContent = question;
      suggestionBtn.addEventListener("click", () => {
        // Set the question as input value
        chatBotInput.value = question;
        // Send the message
        sendMessage();
        // Remove suggestions
        suggestionsContainer.remove();
      });
      suggestionsContainer.appendChild(suggestionBtn);
    });

    chatbotMessages.appendChild(suggestionsContainer);
  }
});

function sendMessage() {
  const userMessage = document.getElementById("chatbot-input").value.trim();
  if (userMessage) {
    appendMessage("user", userMessage);
    document.getElementById("chatbot-input").value = "";
    getBotResponse(userMessage);
  }
}

function appendMessage(sender, message) {
  const messageContainer = document.getElementById("chatbot-messages");
  const messageElement = document.createElement("div");
  messageElement.classList.add("message", sender);
  messageElement.textContent = message;
  messageContainer.appendChild(messageElement);
  messageContainer.scrollTop = messageContainer.scrollHeight;
}

async function getBotResponse(userMessage) {
  const API_KEY = "AIzaSyDPtDlfv-15-Nv_FlbPSRsHV-Bii11ipys";
  const API_URL = `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=${API_KEY}`;

  try {
    // Fetch products and categories data
    const [productsResponse, categoriesResponse] = await Promise.all([
      fetch('http://localhost:5140/api/SanPham'),
      fetch('http://localhost:5140/api/DanhMuc')
    ]);

    const products = await productsResponse.json();
    const categories = await categoriesResponse.json();

    // Create context for the AI
    const context = {
      products: products.map(p => ({
        id: p.id_SanPham,
        name: p.tenSanPham,
        price: p.giaBan,
        salePrice: p.giaKhuyenMai,
        category: p.id_DanhMuc,
        description: p.moTa
      })),
      categories: categories.map(c => ({
        id: c.id_DanhMuc,
        name: c.tenDanhMuc
      }))
    };

    // Add context to the user message
    const prompt = `Context: ${JSON.stringify(context)}\n\nUser question: ${userMessage}\n\nPlease provide a helpful response based on the product and category information provided.`;

    const response = await fetch(API_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        contents: [
          {
            parts: [{ text: prompt }],
          },
        ],
      }),
    });

    const data = await response.json();

    if (!data.candidates || !data.candidates.length) {
      throw new Error("No response from Gemini API");
    }

    const botMessage = data.candidates[0].content.parts[0].text;
    appendMessage("bot", botMessage);
  } catch (error) {
    console.error("Error:", error);
    appendMessage(
      "bot",
      "Xin lỗi, tôi đang gặp vấn đề khi tìm thông tin. Vui lòng thử lại sau."
    );
  }
}
