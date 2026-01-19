/********************************************************************************
 This file contains the variables and functions that form the main game screen of
 the game. The main game progression (dealing, exchanging, revealing, stripping)
 and everything to do with displaying the main game screen.
 ********************************************************************************/

/**********************************************************************
 *****                    Game Screen UI Elements                 *****
 **********************************************************************/

/* main UI elements */
$gameBubbles = [$("#game-bubble-1"),
                $("#game-bubble-2"),
                $("#game-bubble-3"),
                $("#game-bubble-4")];
$gameDialogues = [$("#game-dialogue-1"),
                  $("#game-dialogue-2"),
                  $("#game-dialogue-3"),
                  $("#game-dialogue-4")];
$gameImages = [$("#game-image-1"),
               $("#game-image-2"),
               $("#game-image-3"),
               $("#game-image-4")];
$gameLabels = [$(".game-name-label-0"),
               $("#game-name-label-1"),
               $("#game-name-label-2"),
               $("#game-name-label-3"),
               $("#game-name-label-4")];
$gameOpponentAreas = [$("#game-opponent-area-1"),
                      $("#game-opponent-area-2"),
                      $("#game-opponent-area-3"),
                      $("#game-opponent-area-4")];
$gamePlayerCountdown = $("#player-countdown");
$gameClimaxOverlay = $('#game-climax-overlay');
$gamePlayerClothingArea = $("#player-game-clothing-area, #player-name-label-minimal");
$gamePlayerCardArea = $("#player-game-card-area");
// This is used to apply the current, loser, tied, and revealed-cards classes
$gamePlayerAreas = [$('#player-game-clothing-area, #player-name-label-minimal, #player-game-card-area')].concat($gameOpponentAreas);

/* dock UI elements */
$gameClothingLabel = $("#game-clothing-label");
$gameClothingCells = [$(".player-0-clothing-1"),
                      $(".player-0-clothing-2"),
                      $(".player-0-clothing-3"),
                      $(".player-0-clothing-4"),
                      $(".player-0-clothing-5"),
                      $(".player-0-clothing-6"),
                      $(".player-0-clothing-7"),
                      $(".player-0-clothing-8")];
$mainButton = $("#main-game-button");
$mainButtonText = $("#main-game-button>span");
$autoAdvanceButtons = $("#auto-advance-button-container");
$autoAdvanceProgressBar = $("#auto-advance-progress-bar");
$cardButtons = $gamePlayerCardArea.children('input');
$characterDebugButtons =   [$("#character-debug-button-1"),
                            $("#character-debug-button-2"),
                            $("#character-debug-button-3"),
                            $("#character-debug-button-4")];                          
$devSelectButtons =    [$("#dev-select-button-1"),
                        $("#dev-select-button-2"),
                        $("#dev-select-button-3"),
                        $("#dev-select-button-4")];                
$debugButtons = $('.debug-button');

/* restart modal */
$restartModal = $("#restart-modal");

$logModal = $("#log-modal");
$logContainer = $('#log-container');

gameDisplays = [
    new GameScreenDisplay(1),
    new GameScreenDisplay(2),
    new GameScreenDisplay(3),
    new GameScreenDisplay(4)
];

/**********************************************************************
 *****                   Game Screen Variables                    *****
 **********************************************************************/

/* pseudo constants */
let GAME_DELAY = 800;
let CARD_SUGGEST = false;
let PLAYER_FINISHING_EFFECT = true;
let EXPLAIN_ALL_HANDS = true;
let AUTO_FADE = true;
let MINIMAL_UI = true;
let SHORT_GAME_MODE = false;
const AUTO_ADVANCE_DELAYS = [undefined, 10000, 7000, 4000];

/* game state
 * 
 * First element: text to display on main button to begin the phase
 * Second element: function to call when main button is clicked
 * Third element (optional): whether to automatically hide/show the table (if AUTO_FADE is set)
 * Fourth element: whether the cards are revealed (used in rollback).
 */
const eGamePhase = {
    GAME_START: [ undefined, undefined, undefined, false ], // Dummy phase
    DEAL:      [ "Deal", startDealPhase, true, false ],
    AITURN:    [ "Next", continueDealPhase, true, false ],
    EXCHANGE:  [ undefined, completeExchangePhase, true, null ],
    REVEAL:    [ "Reveal", completeRevealPhase, true, true ],
    PRESTRIP:  [ "Continue", completeContinuePhase, false, true ],
    STRIP:     [ "Strip", completeStripPhase, false, true ],
    FORFEIT:   [ "Masturbate", completeMasturbatePhase, false, true ],
    END_LOOP:  [ undefined, handleGameOver, undefined, true ],
    GAME_OVER: [ "Ending?", function() { actualMainButtonState = false; doEpilogueModal(); }, undefined, false ],
    END_FORFEIT: [ undefined ], // Specially handled; not a real phase. nextGamePhase will never be set to this.
                                // tickForfeitTimers() will always return true in this situation.
};

let gamePhase = null;
let nextGamePhase = null;

var inGame = false;
var currentTurn = 0;
var currentRound = -1;
var previousLoser = -1;
var recentLoser = -1;
var recentWinner = -1;
let recentTied = null;
var gameOver = false;
var actualMainButtonState = false;
var allowAutoAdvance = false;
var autoAdvanceSpeed = 0;
var autoAdvanceProgress = undefined;
var endWaitDisplay = 0;
var showDebug = false;
const chosenDebug = new Set();

var transcriptHistory = [];

/* When going into rollback, we store a RollbackPoint for the most
 * current game state to returnRollbackPoint.  When exiting rollback,
 * we load state from returnRollbackPoint.
 */
var returnRollbackPoint = null;
var currentRollbackIndex = undefined;

/**********************************************************************
 *****                    Start Up Functions                      *****
 **********************************************************************/

/************************************************************
 * Loads all of the content required to display the title
 * screen.
 ************************************************************/
function loadGameScreen () {
    /* reset all of the player's states */
    for (var i = 1; i < players.length; i++) {
        gameDisplays[i-1].reset(players[i]);
    }
    gamePhase = eGamePhase.GAME_START;
    nextGamePhase = null;
    currentRound = -1;
    previousLoser = -1;
    recentLoser = -1;
    gameOver = false;

    chosenDebug.clear();
    updateDebugState(showDebug);
    
    /* randomize start lines for characters using legacy start lines.
     * The updateAllBehaviours() call below will override this for any
     * characters using new-style start lines.
     *
     * Also go ahead and commit any marker updates from selected lines.
     */
    players.forEach(function (p) {
        if (p.chosenState) {
            p.commitBehaviourUpdate();
        }
    }.bind(this));

    saveAllTranscriptEntries();
    updateAllBehaviours(null, null, GAME_START);
    updateBiggestLead();

    /* update the visuals */
    updateAllGameVisuals();

    /* set up the poker library */
    setupPoker();

    /* disable player cards */
    $cardButtons.attr('disabled', true);

    /* Set up strip modal selectors */
    setupStrippingModal();

    /* enable and set up the main button */
    allowProgression(eGamePhase.DEAL);
}

/**********************************************************************
 *****                      Display Functions                     *****
 **********************************************************************/

/************************************************************
 * Updates all of the main visuals on the main game screen.
 ************************************************************/
function updateGameVisual (player) {
    if (inGame) {
        gameDisplays[player-1].update(players[player]);
    }
}

function updateGameVisuals () {
    for (var i = 1; i < players.length; i++) {
        updateGameVisual(i);
    }
}

/************************************************************
 * Updates all of the main visuals on the main game screen.
 ************************************************************/
function updateAllGameVisuals () {
    /* update all opponents */
    for (var i = 0; i < players.length; i++) {
        // This incorrectly sets the player clothing area display to block, but that's corrected by displayHumanPlayerClothing
        $gamePlayerAreas[i].toggle(!!players[i] && !(players[i].out && !players[i].hand)
                                   && !(gameOver && players.every(p => p.hand == null)));
    }
    updateGameVisuals();
    updateHumanPlayerMasturbationVisual();
    displayHumanPlayerClothing();
    displayAllHands(gamePhase[3]);
}

/************************************************************
 * Updates the human player forfeit countdown and finishing effect.
 ************************************************************/
function updateHumanPlayerMasturbationVisual () {
    $gameClimaxOverlay.toggle(PLAYER_FINISHING_EFFECT && humanPlayer.checkStatus(STATUS_HEAVY_MASTURBATING));
    $gameClimaxOverlay.toggleClass('intense', humanPlayer.timer == 1);
    $gamePlayerCountdown.toggle(humanPlayer.out && humanPlayer.timer > 0);
    $gamePlayerCountdown.html(humanPlayer.timer);
    $gamePlayerCountdown.toggleClass('pulse', humanPlayer.checkStatus(STATUS_HEAVY_MASTURBATING));
}

/************************************************************
 * Updates the visuals of the player clothing cells.
 ************************************************************/
function displayHumanPlayerClothing () {
    /* Checking that a player is not only out but also had their hand
     * removed is used to keep their area visible for another phase
     * (until cards dealt, or just another tick at game end) */
    if ((humanPlayer.out && !humanPlayer.hand) || (gameOver && players.every(p => p.hand == null))) {
        $gamePlayerClothingArea.hide();
    } else {
        $gamePlayerClothingArea.css('display', '');
    }

    /* collect the images */
    var clothingImages = humanPlayer.getClothing().map(function(c) {
        return { src: c.image,
                 alt: c.name.initCap() };
    });

    /* display the remaining clothing items */
    clothingImages.reverse();
    /* update label */
    if (humanPlayer.out) {
        $gameClothingLabel.html("You're Masturbating...");
    } else if (humanPlayer.countLayers() > 0) {
        $gameClothingLabel.html("Your Remaining Clothing");
    } else {
        $gameClothingLabel.html("You're Naked");
    }

    for (var i = 0; i < 8; i++) {
        if (clothingImages[i]) {
            $gameClothingCells[i].attr(clothingImages[i]);
            $gameClothingCells[i].parent().css({opacity: 1});
        } else {
            $gameClothingCells[i].parent().css({opacity: 0});
        }
    }
}

/**********************************************************************
 *****                        Flow Functions                      *****
 **********************************************************************/

/************************************************************
 * Determines what the AI's action will be.
 ************************************************************/
function makeAIDecision () {
    detectCheat();
    /* determine the AI's decision */
    determineAIAction(players[currentTurn]);
    
    /* update a few hardcoded visuals */
    players[currentTurn].swapping = true;
    players[currentTurn].singleBehaviourUpdate(SWAP_CARDS);

    /* wait and implement AI action */
    var n = players[currentTurn].hand.tradeIns.countTrue();
    exchangeCards(currentTurn);
    timeoutID = window.setTimeout(reactToNewAICards,
                                  Math.max(GAME_DELAY, n ? (n - 1) * ANIM_DELAY + ANIM_TIME + GAME_DELAY / 4 : 0));
}

/************************************************************
 * React to the new cards
 ************************************************************/
function reactToNewAICards () {
    /* update behaviour */
    if (players[currentTurn].hand.strength == HIGH_CARD) {
        players[currentTurn].singleBehaviourUpdate([BAD_HAND, ANY_HAND]);
    } else if (players[currentTurn].hand.strength == PAIR) {
        players[currentTurn].singleBehaviourUpdate([OKAY_HAND, ANY_HAND]);
    } else {
        players[currentTurn].singleBehaviourUpdate([GOOD_HAND, ANY_HAND]);
    }

    players[currentTurn].swapping = false;

    /* wait and then advance the turn */
    timeoutID = window.setTimeout(advanceTurn, GAME_DELAY / 2);
}

/************************************************************
 * Advances the turn or ends the round.
 ************************************************************/
function advanceTurn () {
    currentTurn++;
    if (currentTurn >= players.length) {
        currentTurn = 0;
    }

    if (players[currentTurn]) {
        /* highlight the player whose turn it is */
        for (var i = 0; i < players.length; i++) {
            $gamePlayerAreas[i].toggleClass('current', currentTurn == i);
        }

        /* check to see if they are still in the game */
        if (players[currentTurn].out && currentTurn > 0) {
            /* update their speech and skip their turn */
            players[currentTurn].singleBehaviourUpdate(players[currentTurn].forfeit[1] == CAN_SPEAK ?
                                                 addTriggers(players[currentTurn].forfeit[0], ANY_HAND) :
                                                       players[currentTurn].forfeit[0]);

            timeoutID = window.setTimeout(advanceTurn, GAME_DELAY);
            return;
        }
    }

    /* allow them to take their turn */
    if (currentTurn == 0) {
        /* Reprocess reactions. */
        updateAllVolatileBehaviours();
        commitAllBehaviourUpdates();
        updateGameVisuals();

        /* human player's turn */
        if (humanPlayer.out) {
            allowProgression(eGamePhase.REVEAL);
        } else {
            $gameScreen.addClass('prompt-exchange');
            allowProgression(eGamePhase.EXCHANGE);
        }
    } else if (!players[currentTurn]) {
        /* There is no player here, move on. */
        advanceTurn();
    } else {
        /* AI player's turn */
        makeAIDecision();
    }
}

/************************************************************
 * Deals cards to each player and resets all of the relevant
 * information.
 ************************************************************/
function startDealPhase () {
    if (currentRound++ < 0) {
        recordStartGameEvent();
    }
    saveTranscriptMessage("Starting round "+(currentRound+1)+"...");

    Sentry.addBreadcrumb({
        category: 'game',
        message: 'Starting round '+(currentRound+1)+'...',
        level: 'info'
    });

    /* dealing cards */
    dealLock = getNumPlayersInStage(STATUS_ALIVE) * CARDS_PER_HAND;
    for (var i = 0; i < players.length; i++) {
        if (players[i]) {
            /* collect the player's hand */
            clearHand(i);
        }
    }

    setupDeck();

    var numPlayers = getNumPlayersInStage(STATUS_ALIVE);

    var n = 0;
    for (var i = 0; i < players.length; i++) {
        if (players[i]) {
            console.log(players[i] + " "+ i);
            if (!players[i].out) {
                /* deal out a new hand to this player */
                dealHand(i, numPlayers, n++);
            } else {
                players[i].hand = null;
                $gamePlayerAreas[i].hide();
            }
        }
    }

    /* IMPLEMENT STACKING/RANDOMIZED TRIGGERS HERE SO THAT AIs CAN COMMENT ON PLAYER "ACTIONS" */

    timeoutID = window.setTimeout(checkDealLock, ANIM_DELAY * CARDS_PER_HAND * numPlayers + ANIM_TIME);
}

/************************************************************
 * Checks the deal lock to see if the animation is finished.
 ************************************************************/
function checkDealLock () {
    /* check the deal lock */
    if (dealLock > 0) {
        timeoutID = window.setTimeout(checkDealLock, 100);
    } else {
        /* Set up main button.  If there is not pause for the human
           player to exchange cards, and someone is masturbating, and
           the card animation speed is to great, we need a pause so
           that the masturbation talk can be read. */
        if (humanPlayer.out && getNumPlayersInStage(STATUS_MASTURBATING) > 0 && ANIM_DELAY < 100) {
            allowProgression(eGamePhase.AITURN);
        } else {
            gamePhase = eGamePhase.AITURN;
            continueDealPhase();
        }
    }
}

/************************************************************
 * Finishes the deal phase and allows the game to progress.
 ************************************************************/
function continueDealPhase () {
    /* hide the dialogue bubbles */
    players.forEach(function (p) {
        if (p.currentState) {
            p.currentState.dialogue = '';
            p.keepPose = true;
            updateGameVisual(p.slot);
        }
    });

    $mainButtonText.html("Wait...");
    
    /* enable player cards */
    $cardButtons.attr('disabled', false);

    /* suggest cards to swap, if enabled */
    if (CARD_SUGGEST && !humanPlayer.out) {
        determineAIAction(humanPlayer);
        
        /* dull the cards they are trading in */
        for (var i = 0; i < humanPlayer.hand.tradeIns.length; i++) {
            if (humanPlayer.hand.tradeIns[i]) {
                dullCard(HUMAN_PLAYER, i);
            }
        }
    }

    /* Clear all player's chosenStates to allow for limited (in-order-only)
     * reaction processing during the AI turns.
     * (Handling of out-of-order reactions happens at the beginning of the
     *  player turn.)
     */
    players.forEach(function (p) {
        if (p.chosenState) {
            p.clearChosenState();
        }
    });

    /* allow each of the AIs to take their turns */
    currentTurn = 0;
    advanceTurn();
}

/************************************************************
 * Processes everything required to complete the exchange phase
 * of a round. Trades in the cards the player has selected and
 * draws new ones.
 ************************************************************/
function completeExchangePhase () {
    detectCheat();
    /* disable player cards */
    $cardButtons.attr('disabled', true);

    /* exchange the player's chosen cards */
    exchangeCards(HUMAN_PLAYER);

    $gamePlayerAreas[HUMAN_PLAYER].removeClass("current");
    allowProgression(eGamePhase.REVEAL);
}

/************************************************************
 * Processes everything required to complete the reveal phase
 * of a round. Shows everyone's hand and determines who lost
 * the hand.
 ************************************************************/
function completeRevealPhase () {
    detectCheat();
    stopCardAnimations(); // If the player was impatient
    /* reveal everyone's hand */
    for (var i = 0; i < players.length; i++) {
        if (players[i] && !players[i].out) {
            players[i].hand.sort();
        }
    }

    /* Sort players by their hand strengths, worst first. */
    var sortedPlayers = players.filter(function(p) { return !p.out; });
    sortedPlayers.sort(function(p1, p2) { return compareHands(p1.hand, p2.hand); });

    if (DEBUG && chosenDebug.size > 1) {
        recentTied = [...chosenDebug];
    } else if (DEBUG && chosenDebug.size == 1) {
        previousLoser = recentLoser;
        recentLoser = chosenDebug.keys().next().value;
        recentTied = null;
    } else {
        /* Check if (at least) the two worst hands are equal. */
        if (compareHands(sortedPlayers[0].hand, sortedPlayers[1].hand) == 0) {
            console.log("Fuck... there was an absolute tie");
            recentTied = [];
            /* The probability of a three-way tie is basically zero,
             * but it's theoretically possible. */
            for (var i = 0;
                 i < 2 || (i < sortedPlayers.length
                           && compareHands(sortedPlayers[0].hand,
                                           sortedPlayers[i].hand) == 0);
                 i++) {
                recentTied.push(sortedPlayers[i].slot);
            };
        } else {
            previousLoser = recentLoser;
            recentLoser = sortedPlayers[0].slot;
            recentTied = null;
        }
    }
    if (recentTied !== null) {
        /* inform the player */
        players.forEach(function (p) {
            if (p.currentState) {
                p.currentState.dialogue = '';
                p.keepPose = true;
                updateGameVisual(p.slot);
            }
        });
        updateAllBehaviours(null, null, PLAYERS_TIED);

        displayAllHands(true);
        /* reset the round */
        allowProgression(eGamePhase.DEAL);
        return;
    }
    recentWinner = sortedPlayers[sortedPlayers.length-1].slot;

    console.log("Player "+recentLoser+" is the loser.");
    Sentry.addBreadcrumb({
        category: 'game',
        message: players[recentLoser].id+' lost the round',
        level: 'info'
    });

    // update loss history
    if (recentLoser == previousLoser) {
        // same player lost again
        players[recentLoser].consecutiveLosses++;
    } else {
        // a different player lost
        players[recentLoser].consecutiveLosses = 1;
        if (previousLoser >= 0) players[previousLoser].consecutiveLosses = 0; //reset last loser
    }

    /* update behaviour */
    var clothes = playerMustStrip (recentLoser);

    /* playerMustStrip() calls updateAllBehaviours. */

    /* Reveal all hands and show the loser */
    displayAllHands(true);

    /* set up the main button */
    if (recentLoser != HUMAN_PLAYER && clothes > 0) {
        allowProgression(eGamePhase.PRESTRIP);
    } else if (clothes == 0) {
        allowProgression(eGamePhase.FORFEIT);
    } else {
        allowProgression(eGamePhase.STRIP);
    }

}

/************************************************************
 * Processes everything required to complete the continue phase
 * of a round. A very short phase in which a player removes an
 * article of clothing.
 ************************************************************/
function completeContinuePhase () {
    /* show the player removing an article of clothing */
    prepareToStripPlayer(recentLoser);
    allowProgression(eGamePhase.STRIP);
}

/************************************************************
 * Processes everything required to complete the strip phase
 * of a round. Makes the losing player strip.
 ************************************************************/
function completeStripPhase () {
    /* strip the player with the lowest hand */
    stripPlayer(recentLoser);
}

/************************************************************
 * Processes everything required to complete the strip phase of a
 * round when the loser has no clothes left. Makes the losing player
 * start their forfeit. May also end the game if only one player
 * remains.
 ************************************************************/
function completeMasturbatePhase () {
    /* strip the player with the lowest hand */
    startMasturbation(recentLoser);
}

/************************************************************
 * Handles everything that happens at the end of the round.
 * Including the checks for the end of game.
 ************************************************************/
function endRound () {
    /* check to see how many players are still in the game */
    var inGame = 0;
    var notInGame = 0;
    var lastPlayer = 0;
    var outPlayer = 0;
    for (var i = 0; i < players.length; i++) {
        if (players[i]) {
            players[i].timeInStage++;
            if (players[i].out) {
                notInGame++;
                outPlayer = i;
            } else {
                inGame++;
                lastPlayer = i;
            }
        }
    }

    /* if there is only one player left, end the game */
    if (inGame <= 1) {
        recordEndGameEvent(players[lastPlayer].id);
        
        console.log("The game has ended!");
        saveTranscriptMessage('<b>' + players[lastPlayer].label.escapeHTML() + "</b> won Strip Poker Night at the Inventory!");
        gameOver = true;

        Sentry.addBreadcrumb({
            category: 'game',
            message: 'Game ended with '+players[lastPlayer].id+' winning.',
            level: 'info'
        });

        endWaitDisplay = 0;
        allowProgression(eGamePhase.END_LOOP);
    } else if (SHORT_GAME_MODE && notInGame > 0) {
        let mostLayersLeft = 0, winner = 0, winners = "";
        for (var i = 0; i < players.length; i++) {
            if (players[i] && players[i].countLayers() > mostLayersLeft) {
                mostLayersLeft = players[i].countLayers();
                winner = i;
            }
            if (players[i] && !players[i].out) {
                if (winners) {
                    winners += " and ";
                }
                winners += players[i].label.escapeHTML();
            }
        }
        recordEndGameEvent(players[winner].id);

        console.log("The game has ended!");

        saveTranscriptMessage('<b>' + winners + "</b> won Strip Poker Night at the Inventory!");
        gameOver = true;

        Sentry.addBreadcrumb({
            category: 'game',
            message: 'Game ended with '+players[winner].id+' winning.',
            level: 'info'
        });

        endWaitDisplay = 0;
        allowProgression(eGamePhase.END_LOOP);
    } else {
        updateBiggestLead();
        allowProgression(eGamePhase.DEAL);
        ACTIVE_CARD_IMAGES.preloadImages();
    }
}

/************************************************************
 * Handles the end of the game. Currently just waits for all
 * players to finish their forfeits.
 ************************************************************/
function handleGameOver() {
    var winner, loser;

    /* determine true end and identify winner (even though endRound() did that too) */
    if (!players.some(function(p) {
        if (!p.out && !SHORT_GAME_MODE) {
            winner = p;
        } else if (p.out && SHORT_GAME_MODE) {
            loser = p;
        }
        return p.checkStatus(STATUS_MASTURBATING);
    })) {
        /* true end */
        if (SHORT_GAME_MODE) {
            updateAllBehaviours(loser.slot, GAME_OVER_DEFEAT, GAME_OVER_VICTORY);
        } else {
            updateAllBehaviours(winner.slot, GAME_OVER_VICTORY, GAME_OVER_DEFEAT);
        }
        allowProgression(eGamePhase.GAME_OVER);
        if (AUTO_FADE && tableVisibility < 0) forceTableVisibility(0);
    } else {
        players.forEach(p => p.hand = null);

        $gamePlayerAreas.forEach(a => a.hide());

        if (endWaitDisplay == 3) {
            players.forEach(function(p) { p.timeInStage++; });
        }
        endWaitDisplay = (endWaitDisplay + 1) % 4;
        allowProgression(eGamePhase.END_LOOP);
    }
}

/**********************************************************************
 *****                    Interaction Functions                   *****
 **********************************************************************/

/************************************************************
 * The player selected one of their cards.
 ************************************************************/
function selectCard (card) {
    if (inRollback()) return;
    humanPlayer.hand.tradeIns[card] = !humanPlayer.hand.tradeIns[card];
    
    if (humanPlayer.hand.tradeIns[card]) {
        dullCard(HUMAN_PLAYER, card);
    } else {
        fillCard(HUMAN_PLAYER, card);
    }
    updateMainButtonExchangeLabel();
}

function updateMainButtonExchangeLabel() {
    if (nextGamePhase === eGamePhase.EXCHANGE) {
        const n = humanPlayer.hand.tradeIns.countTrue();
        $mainButtonText.html(n == 0 ? 'Keep all' : 'Swap ' + n);
    }
}

function getGamePhaseString(phase) {
    var keys = Object.keys(eGamePhase);
    for (var i=0;i<keys.length;i++) {
        if (eGamePhase[keys[i]] === phase) return keys[i];
    }
}

/************************************************************
 * Allow progression by enabling the main button *or*
 * setting up the auto forfeit timer.
 ************************************************************/
function allowProgression (nextPhase) {
    if (nextPhase !== undefined) {
        nextGamePhase = nextPhase;
    }

    if (inRollback()) {
        $mainButtonText.html('Return');
    } else if (humanPlayer.out && !humanPlayer.finished && humanPlayer.timer == 1 && nextGamePhase != eGamePhase.STRIP) {
        $mainButtonText.html("Cum!");
        if (AUTO_FADE) forceTableVisibility(0);
    } else if (justFinishedPlayer() >= 0) {
        $mainButtonText.html("Continue...");
    } else if (nextGamePhase[0]) {
        $mainButtonText.html(nextGamePhase[0]);
    } else if (nextGamePhase === eGamePhase.EXCHANGE) {
        updateMainButtonExchangeLabel();
    } else if (nextGamePhase === eGamePhase.END_LOOP) { // Special case
        /* someone is still forfeiting */
        var dots = '.'.repeat(endWaitDisplay);
        if (humanPlayer.checkStatus(STATUS_MASTURBATING)) {
            $mainButtonText.html("<small>Keep going" + dots + "</small>");
        } else {
            $mainButtonText.html("Wait" + dots);
        }
    }

    actualMainButtonState = false;
    timeoutID = undefined;
    allowAutoAdvance = nextGamePhase != eGamePhase.GAME_OVER && !inRollback()
        && ((humanPlayer.out && (humanPlayer.timer > 1 || nextGamePhase == eGamePhase.STRIP)
             || humanPlayer.finished || (!humanPlayer.out && gameOver)));
    $autoAdvanceButtons.toggle(allowAutoAdvance);

    $mainButton.attr('disabled', false);
    if (allowAutoAdvance && autoAdvanceSpeed) {
        changeAutoAdvance();
    }
    if (!$(document.activeElement).is(':input')) {
        $mainButton.focus();
    }
}

/************************************************************
 * The player clicked the main button on the game screen.
 ************************************************************/
function advanceGame () {    
    /* disable the button to prevent double clicking */
    $mainButton.attr('disabled', actualMainButtonState = true);
    $autoAdvanceProgressBar.stop().hide();
    autoAdvanceProgress = undefined;

    if ($(document.activeElement).attr('disabled')) {
        /* It appears that in Firefox, if the active element gets
         * disabled, it can stop keyboard events from being emitted
         * altogether. So remove that focus. */
        $(document.activeElement).blur();
    }
    
    if (inRollback()) {
        exitRollback();
    } else {
        /* lower the timers of everyone who is forfeiting */
        if (tickForfeitTimers()) return;

        gamePhase = nextGamePhase;
        if (AUTO_FADE && gamePhase[2] !== undefined) {
            forceTableVisibility(gamePhase[2]);
        }
        gamePhase[1]();
    }
}

/************************************************************
 * If Auto-advance is auto-advancing, stop it.
 ************************************************************/
function pauseAutoAdvance () {
    if (inGame) $autoAdvanceProgressBar.stop();
}
/************************************************************
 * If Auto-advance is enabled and we're not currently in the middle of
 * something, set the timeout again by calling AllowProgression().
 ************************************************************/
function resumeAutoAdvance () {
    if (!inGame || inRollback()) return;
    if (autoAdvanceProgress !== undefined) {
        changeAutoAdvance();
    } else if (!actualMainButtonState) {
        allowProgression();
    }
}

function changeAutoAdvance (val) {
    if (val !== undefined) {
        autoAdvanceSpeed = val;
        // Change appearance of buttons
        $autoAdvanceButtons.children().removeAttr('disabled').eq(autoAdvanceSpeed).attr('disabled', true);
        if (actualMainButtonState) return; // Disallow any auto-advance start while the main button is disabled.
    }

    if (autoAdvanceProgress !== undefined) {  // We are currently auto-advancing
        if (val !== undefined) $autoAdvanceProgressBar.stop();
        if (autoAdvanceSpeed == 0) {
            // Reset, return to manual advance
            autoAdvanceProgress = undefined;
            $autoAdvanceProgressBar.hide();
            return;
        }
    } else if (val && !actualMainButtonState) {
        // When activating auto advance, immediately advance one phase.
        advanceGame();
        return;
    } else if (autoAdvanceSpeed > 0) {
        // Starting an auto-advance timeout. We should be called from
        // allowProgress() with val undefined here, so show the progress
        // bar.
        autoAdvanceProgress = 0;
        $autoAdvanceProgressBar.width(0).show();
    }
    if (autoAdvanceSpeed) {
        // Start or restart animation
        $autoAdvanceProgressBar.animate(
            { width: '100%' },
            { duration: AUTO_ADVANCE_DELAYS[autoAdvanceSpeed] * (1 - autoAdvanceProgress),
              easing: 'linear',
              progress: function (anim, progress, remaining) {
                  autoAdvanceProgress = 1 - remaining / AUTO_ADVANCE_DELAYS[autoAdvanceSpeed];
              },
              complete: function () {
                  advanceGame();
              },
            });
    }
}

/************************************************************
 * The player clicked the home button. Shows the restart modal.
 ************************************************************/
function showRestartModal () {
    $restartModal.modal('show');
}

/************************************************************
 * Functions and classes for the dialogue transcript and rollback.
 ************************************************************/

/************************************************************
 * Encapsulates character state info for rollback.
 * A lot of this data isn't actually user-visible,
 * but is kept to support accurate submission of bug reports
 * from a rolled-back state.
 ************************************************************/
function RollbackPoint (logPlayers) {
    this.playerData = players.map(function (p) {
        const data = {};
        
        data.slot = p.slot;
        data.stage = p.stage;
        data.poseStage = p.poseStage;
        data.folder = p.folder;
        data.poses = p.poses;
        data.poseSets = p.poseSets;
        data.timeInStage = p.timeInStage;
        data.ticksInStage = p.ticksInStage;
        
        data.markers = {};
        for (let marker in p.markers) {
            data.markers[marker] = p.markers[marker];
        }
        
        if (p.currentState) data.currentState = new State(p.currentState);
        data.keepPose = p.keepPose;

        if (p.hand) data.hand = p.hand.clone(); else data.hand = p.hand;

        data.label = p.label;
        // These probably only matter for the human player
        data.timer = p.timer;
        data.forfeit = p.forfeit?.slice();
        data.out = p.out;
        data.finished = p.finished;
        if (p.clothing) {
            data.clothingRemovalStatus = p.clothing.map(c => c.removed);
        }

        return data;;
    });
    
    /* Record data for bug reporting purposes. */
    this.currentRound = currentRound;
    this.currentTurn = currentTurn;
    this.previousLoser = previousLoser;
    this.recentLoser = recentLoser;
    this.recentWinner = recentWinner;
    this.recentTied = recentTied;
    this.gameOver = gameOver;
    this.gamePhase = gamePhase;
    
    this.logEntries = [];
    
    if (logPlayers) {
        logPlayers.forEach(function (p) {
            if (!players[p]) return;
            
            this.logEntries.push([
                players[p].label,
                players[p].currentState ? players[p].currentState.dialogue : ''
            ]);
        }.bind(this));
    }
}

RollbackPoint.prototype.load = function () {
    currentRound = this.currentRound;
    currentTurn = this.currentTurn;
    previousLoser = this.previousLoser;
    recentLoser = this.recentLoser;
    recentWinner = this.recentWinner;
    recentTied = this.recentTied;
    gameOver = this.gameOver;
    gamePhase = this.gamePhase;
    if (this.nextGamePhase) {
        nextGamePhase = this.nextGamePhase;
    }
    
    this.playerData.forEach(function (p) {
        var loadPlayer = players[p.slot];
        
        loadPlayer.stage = p.stage;
        loadPlayer.poseStage = p.poseStage;
        loadPlayer.folder = p.folder;
        loadPlayer.poses = p.poses;
        loadPlayer.poseSets = p.poseSets;
        loadPlayer.timeInStage = p.timeInStage;
        loadPlayer.ticksInStage = p.ticksInStage;
        loadPlayer.markers = p.markers;
        loadPlayer.currentState = p.currentState;
        loadPlayer.keepPose = p.keepPose;
        loadPlayer.timer = p.timer;
        loadPlayer.forfeit = p.forfeit;
        loadPlayer.out = p.out;
        loadPlayer.finished = p.finished;
        loadPlayer.label = p.label;
        loadPlayer.hand = p.hand;
        loadPlayer.clothing.forEach((c, i) => { c.removed = p.clothingRemovalStatus ? p.clothingRemovalStatus[i] : true });
        /* Because the rollback point will have been created after the
         * first stage change, if in the STRIP phase, we need to redo any
         * stage skips before updating the visuals. */
        let skipToStage = loadPlayer.findNextRealStage();
        if (skipToStage) {
            loadPlayer.stage = skipToStage;
            loadPlayer.stageChangeUpdate();
        }
    }.bind(this));
}

function inRollback() {
    return (!!returnRollbackPoint);
}

function loadRollbackPoint(pt) {
    if (!returnRollbackPoint) {
        returnRollbackPoint = new RollbackPoint();
        returnRollbackPoint.nextGamePhase = nextGamePhase;
        $cardButtons.attr('disabled', true);
        changeAutoAdvance(0);
        $('.transcript-step-button').show();
    }

    Sentry.addBreadcrumb({
        category: 'ui',
        message: 'Entering rollback.',
        level: 'info'
    });

    pt.load();
    updateAllGameVisuals();
    if (AUTO_FADE && gamePhase[2] !== undefined) {
        forceTableVisibility(gamePhase[2] && players.some(p => p.hand));
    }
    allowProgression();
}

function stepThroughTranscript(step) {
    if (!inRollback()) return;
    do {
        currentRollbackIndex += step;
    } while (currentRollbackIndex > 0 && currentRollbackIndex < transcriptHistory.length - 1
             && !(transcriptHistory[currentRollbackIndex] instanceof RollbackPoint));
    loadRollbackPoint(transcriptHistory[currentRollbackIndex]);
    $('.transcript-step-button.left').toggle(currentRollbackIndex > 0);
    $('.transcript-step-button.right').toggle(currentRollbackIndex < transcriptHistory.length - 1);
}

function exitRollback() {
    if (!inRollback()) return;

    Sentry.addBreadcrumb({
        category: 'ui',
        message: 'Exiting rollback.',
        level: 'info'
    });

    /* We first load the last saved transcript entry only because when
     * returning to a FORFEIT phase, a character that's just started
     * masturbating will have been in the stage after the
     * start_masturbating case when the return rollback point was
     * created.
     (Should no longer be needed)
    transcriptHistory.findLast(e => e instanceof RollbackPoint).load(); */
    updateAllGameVisuals();
    returnRollbackPoint.load();
    returnRollbackPoint = null;
    currentRollbackIndex = undefined;
    $('.transcript-step-button').hide();
    allowProgression();
    $cardButtons.attr('disabled', nextGamePhase != eGamePhase.EXCHANGE);
    if (nextGamePhase == eGamePhase.EXCHANGE) {
        humanPlayer.hand.tradeIns.forEach(function(v, i) {
            $cardCells[HUMAN_PLAYER][i].toggleClass('tradein', v);
        });
    }
}

/* Adds a log message to the dialogue transcript */
function saveTranscriptMessage(msg) {
    transcriptHistory.push(msg)
}

/* Creates a rollback point associated with a specified list of players. */
function saveTranscriptEntries(players) {
    transcriptHistory.push(new RollbackPoint(players));
}
 

/* Creates a rollback point associated with a single entry in the dialogue
 * transcript.
 */
function saveSingleTranscriptEntry (player) {
    saveTranscriptEntries([player]);
}

/* Creates a rollback point associated with entries for all players
 * in the dialogue transcript.
 */
function saveAllTranscriptEntries () {
    saveTranscriptEntries([1, 2, 3, 4]);
}

 
/************************************************************
 * Creates a DOM element for a log entry.
 ************************************************************/
function createLogEntryElement(label, text, pt, idx) {
    var container = document.createElement('div');
    container.className = "log-entry-container clearfix";
    
    var labelElem = document.createElement('b');
    labelElem.className = "log-entry-label";
    $(labelElem).html(label);
    
    var textElem = document.createElement('div');
    textElem.className = "log-entry-text";
    $(textElem).html(text);
    
    container.appendChild(labelElem);
    container.appendChild(textElem);
    
    container.onclick = function (ev) {
        if (!actualMainButtonState) {
            loadRollbackPoint(pt);
            currentRollbackIndex = idx;
            $('.transcript-step-button.left').toggle(idx > 0);
            $('.transcript-step-button.right').toggle(idx < transcriptHistory.length - 1);
            $logModal.modal('hide');
        }
    }
    
    return container;
}

/************************************************************
* Creates a DOM element for a logged game message.
************************************************************/
function createLogMessageElement(text) {
    var container = document.createElement('div');
    container.className = "log-entry-container clearfix";
    container.style = "opacity: 1; text-align:center;";
    
    var textElem = document.createElement('i');
    textElem.className = "log-entry-message";
    $(textElem).html(text);
    
    container.appendChild(textElem);
    
    return container;
}

/************************************************************
 * The player clicked the log button. Shows the log modal.
 ************************************************************/
function showLogModal () {
    $logContainer.empty();
    
    transcriptHistory.forEach(function (pt, idx) {
        if (pt instanceof RollbackPoint) {
            pt.logEntries.forEach(function (e) {
                logText = fixupDialogue(e[1]);
                logText = logText.replace(/<script>.+<\/script>/g, '');
                logText = logText.replace(/<button[^>]+>[^<]+<\/button>/g, '');
        
                $logContainer.append(createLogEntryElement(e[0], logText, pt, idx));
            });
        } else {
            $logContainer.append(createLogMessageElement(pt));
        }
    });

    $logModal.modal('show');
}

$('#restart-modal,#log-modal,#bug-report-modal,#feedback-report-modal,#options-modal,#help-modal,#character-debug-modal')
    .on('show.bs.modal', pauseAutoAdvance)
    .on('hidden.bs.modal', resumeAutoAdvance);

/************************************************************
 * A keybound handler.
 ************************************************************/
function game_keyUp(e)
{
    console.log(e);
    if ($('.modal:visible').length == 0 && $('#game-screen .dialogue-edit:visible').length == 0) {
        if (e.key == ' ' && !$mainButton.prop('disabled')
            && !($('body').hasClass('focus-indicators-enabled') && $(document.activeElement).is('button:visible:enabled, input:visible:enabled'))) {
            e.preventDefault();
            advanceGame();
        }
        else if (e.key >= '1' && e.key <= '5' && !$cardButtons.eq(e.key - 1).prop('disabled')) {
            selectCard(e.key - 1);
        }
        else if (e.key.toLowerCase() == 'q' && DEBUG) {
            showDebug = !showDebug;
            updateDebugState(showDebug);
        }
        else if (e.key.toLowerCase() == 't') {
            toggleTableVisibility();
        }
        else if (e.key == '-' && allowAutoAdvance && autoAdvanceSpeed > 0) {
            changeAutoAdvance(autoAdvanceSpeed - 1);
        }
        else if (e.key == '+' && allowAutoAdvance && autoAdvanceSpeed < AUTO_ADVANCE_DELAYS.length - 1) {
            changeAutoAdvance(autoAdvanceSpeed + 1);
        }
    }
}
$gameScreen.data('keyhandler', game_keyUp);

function selectDebug(ev) {
    const player = Number($(ev.target).data('player'));

    // If Ctrl is held or the button of the single selected player is clicked, toggle
    if (ev.ctrlKey || (chosenDebug.size == 1 && chosenDebug.has(player))) {
        if (chosenDebug.has(player)) {
            chosenDebug.delete(player);
        } else {
            chosenDebug.add(player);
        }
    } else { // else select the clicked player
        chosenDebug.clear();
        chosenDebug.add(player);
    }
    updateDebugState(showDebug);
}
$debugButtons.on('click', selectDebug);

function updateDebugState(show)
{
    if (!show) {
        $('.character-debug-button').hide();
        $('.dev-select-button').hide();
        $('.debug-button').hide();
    }
    else {
        $('.character-debug-button').show();
        $('.dev-select-button').show();
        $('.debug-button').show();

        $debugButtons.each(function() {
            const i = $(this).data('player');
            if (!players[i] || players[i].out) {
                $(this).hide();
            } else {
                $(this).toggleClass("active", chosenDebug.has(i));
            }
        });

        for (var i = 0; i < $devSelectButtons.length; i++) {
            if (!players[i + 1])
            {
                $devSelectButtons[i].hide();                
                $characterDebugButtons[i].hide();
            }
        }
    }
}

function detectCheat() {
    /* detect common cheating attempt */
    if (humanPlayer.hand && humanPlayer.hand.cards[0] === "clubs10") {
        players.forEach(function(p, i) {
            if (i == 0) {
                players[i].hand.cards = [ 7, 5, 4, 3, 2 ].map(function(n, i) { return new Card(i % 4, n); });
            } else {
                players[i].hand.cards = [ 14, 13, 12, 11, 10 ].map(function(n) { return new Card(i - 1, n); });
            }
        });
        $('button[onclick^=makeLose]').remove();
        delete makeLose;
        alert("Sorry, but you tried to use an outdated cheat, which would previously have crashed the game. Instead, you will now lose the round.\n\n"+
              "We will now refer you to the FAQ for a few basic strategy tips and information on how to cheat in the proper manner.");
        gotoHelpPage(3);
        showHelpModal();
    }
}
