export { createDiv, createLi }

const createDiv = (innerHTML, classList) => createElement("div", innerHTML, classList);
const createLi = (innerHTML, classList) => createElement("li", innerHTML, classList);







function createElement(tag, innerHTML, classList) {
    var element = document.createElement(tag);
    element.innerHTML = innerHTML;
    if (classList)
        element.classList = classList;
    return element;
}