document.addEventListener("DOMContentLoaded", () => {
  const gamesContainer = document.getElementById("gamesContainer");
  GETFromServer();

  function GETFromServer() {
    const api = "https://localhost:7287/api/Games";
    console.log(api);
    console.log("check");
    ajaxCall("GET", api, "", GETGamesSCB, GETGamesECB);
  }

  function GETGamesSCB(GameList) {
    console.log(GameList);
    gamesContainer.innerHTML = "";
    GameList.forEach((game) => {
      const gameCard = document.createElement("div");
      gameCard.classList.add("game-card");

      const fullText = game.description;
      const maxLength = 100; // מספר התווים המקסימלי להצגה ראשונית
      let truncatedText = fullText;

      if (fullText.length > maxLength) {
        truncatedText = fullText.slice(0, maxLength) + "...";
      }

      gameCard.innerHTML = `
        <img src="${game.headerImage}" alt="${game.name}">
        <h3>${game.name}</h3>
        <h3>${game.appID}</h3>
        <p><strong>Release Date:</strong> ${game.releaseDate}</p>
        <p><strong>Price:</strong> ${game.price}</p>
        <p class="scoreRank"><strong>Rank:</strong>${game.scoreRank}</p>
        <div class="NumOfPurc">
          <img src="../images/purc.png" alt="Medal" class="medal-icon">
          <strong>Purchases:</strong> ${game.numberOfPurchases}
       </div>        
  <p class="description">${truncatedText}</p>
        <a href="${game.Website}" target="_blank">Visit Website</a>
      `;

      // הוספת כפתור "Read More" ו-"Show Less" אם התיאור ארוך
      if (fullText.length > maxLength) {
        const readMoreBtn = document.createElement("button");
        readMoreBtn.textContent = "Read More";
        readMoreBtn.classList.add("read-more-btn");

        const showLessBtn = document.createElement("button");
        showLessBtn.textContent = "Show Less";
        showLessBtn.classList.add("show-less-btn");
        showLessBtn.style.display = "none"; // הכפתור מוסתר בהתחלה

        // מאזין לאירוע לחיצה על "Read More"
        readMoreBtn.addEventListener("click", () => {
          const description = gameCard.querySelector(".description");
          description.textContent = fullText; // הצגת הטקסט המלא
          readMoreBtn.style.display = "none"; // הסתרת "Read More"
          showLessBtn.style.display = "inline-block"; // הצגת "Show Less"
        });

        // מאזין לאירוע לחיצה על "Show Less"
        showLessBtn.addEventListener("click", () => {
          const description = gameCard.querySelector(".description");
          description.textContent = truncatedText; // חזרה לטקסט המקוצר
          showLessBtn.style.display = "none"; // הסתרת "Show Less"
          readMoreBtn.style.display = "inline-block"; // הצגת "Read More"
        });

        gameCard.appendChild(readMoreBtn);
        gameCard.appendChild(showLessBtn);
      }

      // הוספת כפתור "ADD TO MY LIST" מתחת לכפתורים הקיימים
      const addToListBtn = document.createElement("button");
      addToListBtn.textContent = "ADD TO MY LIST";
      addToListBtn.classList.add("btnADD");
      gameCard.appendChild(addToListBtn);

      gamesContainer.appendChild(gameCard);
    });
  }

  function GETGamesECB(err) {
    console.log(err);
  }
});
