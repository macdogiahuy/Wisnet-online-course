import { signInPage } from "../constants.js";
export { createDiv, createLi, redirectToSignin }

const createDiv = (innerHTML, classList) => createElement("div", innerHTML, classList);
const createLi = (innerHTML, classList) => createElement("li", innerHTML, classList);

function redirectToSignin() {
    redirect(signInPage);
}







function createElement(tag, innerHTML, classList) {
    var element = document.createElement(tag);
    element.innerHTML = innerHTML;
    if (classList)
        element.classList = classList;
    return element;
}

function redirect(href) {
    window.location.href = href;
}