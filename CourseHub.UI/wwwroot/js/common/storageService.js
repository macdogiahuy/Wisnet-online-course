import { baseAddress } from '/js/constants.js';
import { getParsedJson } from '/js/apis/http.js';
import { setToLocalStorage, getFromLocalStorageAsJson } from '/js/common/storage.js';

export { getMinGroupsAsync }



const groupsKey = 'groups';

async function getMinGroupsAsync(ids) {
    var arr = getFromLocalStorageAsJson(groupsKey);

    var notIncluded;
    if (!arr) {
        arr = [];
        notIncluded = ids;
    }
    else {
        notIncluded = ids.filter(id => !arr.some(_ => _.id == id));
    }

    if (notIncluded.length > 0) {
        var json = await getParsedJson(
            baseAddress + "/api/conversations/multiple?" +
            notIncluded.map(id => `ids=${id}`).join('&'));

        arr = arr.concat(json);
        setToLocalStorage(groupsKey, JSON.stringify(arr));
    }
    return arr;
}