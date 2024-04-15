document.addEventListener('DOMContentLoaded', function () {
    const authButton = document.getElementById("auth-button");
    if (authButton) authButton.addEventListener('click', authButtonClick);

    initAdminPage();
});

function authButtonClick() {
    const authEmail = document.getElementById("auth-email");
    if (!authEmail) throw "Element '#authEmail' not found!"
    const authPassword = document.getElementById("auth-password");
    if (!authPassword) throw "Element '#authPassword' not found!"
    const authMessage = document.getElementById("auth-message");
    if (!authMessage) throw "Element '#authMessage' not found!"

    const email = authEmail.value.trim();
    if (!email) {
        authMessage.classList.remove('visually-hidden');
        authMessage.innerText = "Hеобхідно ввести E-mail";
        return;
    }
    const password = authPassword.value;

    fetch(`/api/auth?e-mail=${email}&password=${password}`)
        .then(r => {
            if (r.status != 200) {
                authMessage.classList.remove('visually-hidden');
                authMessage.innerText = "Вхід скасовано, перевірте введені дані";
            }
            else {
                // За вимогами безпеки зміна стагусу авторизації потребує перезавантаження
                window.location.reload();
            }
        });
}



///// ADMIN PAGE //////
function initAdminPage() {
    loadCategories();
}
function loadCategories() {
    const container = document.getElementById("category-container");
    if (!container) return;
    fetch("/api/category")      // запитуємо наявні категорії
        .then(r => r.json())
        .then(j => {
            let html = "";
            for (let ctg of j) {
                html += "<p>" + ctg["name"] + "</p>";
            }
            html += `Назва: <input id="ctg-name" /><br/>
            Опис: <textarea id="ctg-description"></textarea><br/>
            <button onclick='addCategory()'>+</button>`;
            container.innerHTML = html;
        });

}
function addCategory() {
    const ctgName = document.getElementById("ctg-name").value;
    const ctgDescription = document.getElementById("ctg-description").value;
    if (confirm(`Додаємо категорію ${ctgName} ${ctgDescription} ?`)) {
        fetch("/api/category", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                'name': ctgName,
                'description': ctgDescription
            })
        })
            .then(r => {
                if (r.status == 201) {
                    loadCategories();
                }
                else {
                    alert("error");
                }
            });
    }
}