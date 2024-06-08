// ui elements
$titleCandy = [$("#left-title-candy"), $("#right-title-candy")];

// global variables
var candyCatalog = [];
var candyTotalWeight = 0;
var candyTransform = ["translateX(-50%)", "translateX(50%)"];

/**
 * Randomly selects two candy images for the title screen. If no candy image
 * options are currently loaded, it will load them from XML first.
 * @returns {void}
 */
function selectTitleCandy () {
    console.log("Selecting title candy...");

    if (!candyCatalog || candyCatalog.length <= 0) {
        loadCandyXML().then(() => randomizeTitleCandy());
    } else {
        randomizeTitleCandy();
    }
}

/**
 * Loads a list of candy elements from a jQuery XML element.
 * @param {jQuery} $xml 
 * @returns {Array<Object>}
 */
function parseCandyCatalog($xml) {
    return $xml.find("candy").map((i, item) => {
        let $item = $(item);
        let candy = {
            uid: $item.attr("uid"),
            uids: $item.attr("uids")?.split(","),
            base: $item.attr("base"),
            src: $item.attr("src"),
            scale: parseFloat($item.attr("scale")),
            weight: parseFloat($item.attr("weight")),
            slot: parseInt($item.attr("slot")),
        };

        candy.weight = candy.weight ? candy.weight : 1.0;

        if (candy.slot) {
            // double the weight of any candy that has a slot restriction
            // otherwise it would be half as likely than normal to appear 
            candy.weight *= 2;
        }

        if (candy.uid && candy.uids && !candy.uids.includes(candy.uid)) {
            candy.uids.push(candy.uid);
        } else if (candy.uid && !candy.uids) {
            candy.uids = [candy.uid];
        }

        return candy;
    }).get();
}

/**
 * Sets the active candy catalog.
 * @param {Array<Object>} catalog 
 */
function setCandyCatalog(catalog) {
    candyCatalog = catalog;
    candyTotalWeight = catalog.reduce((acc, candy) => acc + candy.weight, 0.0);
}

/**
 * Loads the candy image catalog from XML.
 * @returns {Promise<jQuery>}
 */
function loadCandyXML () {
    return fetchXML("opponents/candy.xml").then($xml => {
        setCandyCatalog(parseCandyCatalog($xml));
    });
}

/**
 * Randomly selects and places two unique title candy images.
 * @returns {void}
 */
function randomizeTitleCandy () {
    let candySpaces = 2;
    let currentCatalog = candyCatalog.slice();

    for (let i = 0; i < candySpaces && currentCatalog.length > 0; i++) {
        // filter the catalog to include only candy that accepts this slot index
        let slotWeight = 0;
        let slotCatalog = currentCatalog.filter((candy) => {
            if (!candy.slot || candy.slot === i) {
                slotWeight += candy.weight ? candy.weight : 1.0;
                return true;
            }
            return false;
        });

        // generate a random weight based on the current weight of the catalog
        let random = getRandomNumber(0, slotWeight);

        // find the candy matching the given random weight
        let track = 0;
        let choice = slotCatalog.find((candy, index) => {
            track += candy.weight ? candy.weight : 1.0;
            if (track > random) {
                return candy;
            }
        });

        // if a no candy was selected then skip this slot and move on
        if (!choice) continue;
        placeTitleCandy(i, choice);

        // if more candy is to be selected, filter the catalog to avoid duplicates
        if (i < candySpaces - 1) {
            currentCatalog = currentCatalog.filter((candy) => {
                return !choice.uids.some((uid) => candy.uids.includes(uid));
            });
        }
    }
}

/**
 * Places a candy in the requested title candy slot.
 * @param {number} index The index of the title candy slot to place the candy in.
 * @param {object} candy The candy to place.
 * @returns {void}
 */
function placeTitleCandy (index, candy) {
    let scale = candy.scale ? candy.scale : 1.0;
    let base = "opponents/" + (candy.base ? candy.base : candy.uid) + "/";

    $titleCandy[index].attr("src", base + candy.src);
    $titleCandy[index].css("transform", "scale(" + scale + ") " + candyTransform[index]);
}
