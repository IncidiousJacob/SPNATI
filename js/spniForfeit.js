/********************************************************************************
 This file contains the variables and functions that allow for players to have
 multiple potential forfeits.
 ********************************************************************************/
 
/**********************************************************************
 *****                 Forfeit Object Specification               *****
 **********************************************************************/

var CAN_SPEAK = true;
var CANNOT_SPEAK = false;
 
/**********************************************************************
 *****                      Forfeit Variables                     *****
 **********************************************************************/
 
/* orgasm timer */
const ORGASM_DELAY = 2500;

/* The earliest and latest a character starts heavy masturbation, counted in phases before they finish */
const HEAVY_EARLIEST_TIME = 5;
const HEAVY_LATEST_TIME = 3;

var globalSavedTableVisibility;

/**********************************************************************
 *****                      Forfeit Functions                     *****
 **********************************************************************/

/** 
 * Resolves the specific dialogue trigger to use for forfeit situations based on
 * this character's gender and futanari status.
 * 
 * @param {"must_masturbate" | "start_masturbating" | "masturbating" | "heavy_masturbating" | "finished_masturbating"} triggerType
 * @returns {string}
 */
Player.prototype.getForfeitTrigger = function (triggerType) {
    if (this.gender === eGender.MALE) {
        switch (triggerType) {
            case "must_masturbate": return MALE_MUST_MASTURBATE;
            case "start_masturbating": return MALE_START_MASTURBATING;
            case "masturbating": return MALE_MASTURBATING;
            case "heavy_masturbating": return MALE_HEAVY_MASTURBATING;
            case "finished_masturbating": return MALE_FINISHED_MASTURBATING;
            default: return null;
        }
    } else if (this.gender === eGender.FEMALE) {
        if (!this.penis) {
            switch (triggerType) {
                case "must_masturbate": return FEMALE_MUST_MASTURBATE;
                case "start_masturbating": return FEMALE_START_MASTURBATING;
                case "masturbating": return FEMALE_MASTURBATING;
                case "heavy_masturbating": return FEMALE_HEAVY_MASTURBATING;
                case "finished_masturbating": return FEMALE_FINISHED_MASTURBATING;
                default: return null;
            }
        } else {
            switch (triggerType) {
                case "must_masturbate": return FUTA_MUST_MASTURBATE;
                case "start_masturbating": return FUTA_START_MASTURBATING;
                case "masturbating": return FUTA_MASTURBATING;
                case "heavy_masturbating": return FUTA_HEAVY_MASTURBATING;
                case "finished_masturbating": return FUTA_FINISHED_MASTURBATING;
                default: return null;
            }
        }
    }
}


 /**
  * Update this player's heavy masturbation status and return it.
  *
  * This method will set the player to heavy masturbation if the appropriate
  * prerequisites are met: for AI players, this depends on random chance;
  * for human players, this checks the timer value against a constant value.
  *
  * This does nothing for players that aren't masturbating (i.e. still in the game
  * or already finished) and players whose forfeit statuses are locked by dialogue operations.
  *
  * In all cases, we return whether or not the player is heavily masturbating.
  *
  * @returns {boolean}
 */
Player.prototype.updateHeavyMasturbation = function () {
    if (this.finished || !this.out) return false;

    if (this.slot == HUMAN_PLAYER) {
        /* Human player: go into heavy masturbation at 5 ticks left. */
        this.forfeit[0] = (this.timer <= 4) ? PLAYER_HEAVY_MASTURBATING : PLAYER_MASTURBATING;
    } else if (!this.forfeitLocked) {
        /* AI player: roll random chance they go into heavy masturbation. */
        if (this.timer > HEAVY_EARLIEST_TIME) {
            this.forfeit = [PLAYER_MASTURBATING, CAN_SPEAK];
        } else if (this.timer <= getRandomNumber(HEAVY_LATEST_TIME, HEAVY_EARLIEST_TIME + 1)) {
            this.forfeit = [PLAYER_HEAVY_MASTURBATING, CANNOT_SPEAK];
        }
    }

    /* Players with locked forfeit status fall through with no changes. */

    return this.forfeit[0] == PLAYER_HEAVY_MASTURBATING;
}

/************************************************************
 * Initiate masturbation for the selected player.
 * In the future, we might want to make this a method of Player.
 ************************************************************/
function startMasturbation (player) {
    players[player].forfeit = [PLAYER_MASTURBATING, CAN_SPEAK];
    players[player].forfeitLocked = false;
    players[player].finishingTarget = players[player];
    players[player].out = true;
    players[player].outOrder = players.countTrue(function(p) { return p.out; });

    if (chosenDebug === player || (chosenDebug instanceof Set && chosenDebug.has(player))) {
        chosenDebug = null;
        updateDebugState(showDebug);
    }

    players[player].ticksInStage = 0;

    /* Set timer before playing dialogue, so that forfeitTimer conditions work properly. */
    players[player].timer = players[player].stamina;

    /* update behaviour */
    updateAllBehaviours(
        player, 
        PLAYER_START_MASTURBATING,
        [[players[player].getForfeitTrigger("start_masturbating"), OPPONENT_START_MASTURBATING]]
    );

    /* Note: It's a bit of a problem and an exception that the stage
       is incremented after the dialogue update (and thus
       start_masturbating happens at the end of the naked stage rather
       than at the start of the masturbating stage, because the
       character's current stage then doesn't match the dialogue on
       the screen, complicating rollback and bug reports, but we can't
       easily change this. */
    players[player].stage += 1;
    players[player].timeInStage = -1;
    players[player].stageChangeUpdate();
    
    if (player == HUMAN_PLAYER) {
        updateHumanPlayerMasturbationVisual();
        displayHumanPlayerClothing();
    }
    
    /* allow progression */
    endRound();
}

/************************************************************
 * Check if any character has just finished and needs their
 * finished_masturbating dialogue to play.
 ************************************************************/
function justFinishedPlayer () {
    return players.findIndex(p => p && p.out && !p.finished && p.timer == 0);
}

/************************************************************
 * The forfeit timers of all players tick down, if they have 
 * been set.
 ************************************************************/
function tickForfeitTimers () {
    console.log("Ticking forfeit timers...");
    
    const finishedPlayer = justFinishedPlayer();
    if (finishedPlayer >= 0) {
        finishMasturbation(finishedPlayer);
        return true;
    }
    let masturbatingPlayers = [], heavyMasturbatingPlayers = [];

    if (nextGamePhase != eGamePhase.STRIP && gamePhase != eGamePhase.FORFEIT) for (var i = 0; i < players.length; i++) {
        if (players[i] && players[i].out && players[i].timer == 1) {
            players[i].timer = 0;
            players[i].ticksInStage++;
            gamePhase = eGamePhase.END_FORFEIT;
            /* set the button state */
            $mainButtonText.html("Cumming...");

            saveTranscriptMessage('<b>' + players[i].label.escapeHTML() + '</b> is finishing...');
            console.log(players[i].label+" is finishing!");
            if (i == HUMAN_PLAYER) {
                /* Hide everyone's dialogue bubbles. */
                gameDisplays.forEach(function (d) {
                    d.hideBubble();
                });

                /* player's timer is up */
                /* TEMP FIX: prevent this animation on Safari */
                $gamePlayerCountdown.hide();
                if (PLAYER_FINISHING_EFFECT) {
                    $gameClimaxOverlay.one('animationend', finishMasturbation.bind(null, i))
                        .addClass('climax');
                } else {
                    finishMasturbation(i);
                }
                $gameClothingLabel.html("<b>You're 'Finished'</b>");

            } else {
                let finishTarget = players[i].finishingTarget;

                // Clear all dialogue, like for a tie, so all other character are silent.
                players.forEach(function (p) {
                    if (p.chosenState) {
                        p.chosenState.dialogue = '';
                        if (p != finishTarget) updateGameVisual(p.slot);
                    }
                });
                /* let the player speak again */
                players[i].forfeit = [PLAYER_FINISHING_MASTURBATING, CAN_SPEAK];

                /* show them cumming */
                if (finishTarget && finishTarget.slot !== i) {
                    /* If the player has redirected their finishing dialogue to another character,
                     * play Opponent Finishing dialogue for them.
                     */
                    finishTarget.singleBehaviourUpdate(OPPONENT_FINISHING_MASTURBATING, players[i]);
                } else {
                    players[i].singleBehaviourUpdate(PLAYER_FINISHING_MASTURBATING);
                }

                /* Make sure the character's timer is _actually_ zero, in case they try to do something like
                 * reset their forfeit timer as part of a Finishing line.
                 * Question: do we actually want to disallow this?
                 */
                players[i].timer = 0;

                /* trigger the callback */
                var player = i;
                timeoutID = window.setTimeout(function() { allowProgression(); },
                                              ORGASM_DELAY
                                              + (allowAutoAdvance && autoAdvanceSpeed ?
                                              /* When auto advance active, make the total time until the next phase the
                                                 maximum of the auto advance delay and the animation time, plus an extra
                                                 ORGASM_DELAY, i.e. if the animation is longer than the auto advance delay,
                                                 pause that much longer. */
                                                 Math.max(gameDisplays[finishTarget.slot - 1].animationDuration() - AUTO_ADVANCE_DELAYS[autoAdvanceSpeed], 0) :
                                                 0));
                globalSavedTableVisibility = tableVisibility;
                if (AUTO_FADE) forceTableVisibility(false);
            }
            return true;
        }
    }

    /* Always update ticksInStage for all players, even if they're not out. */
    players.forEach(function (p) {
        p.ticksInStage++;
    });

    for (var i = 0; i < players.length; i++) {
        if (players[i] && players[i].out && !players[i].finished) {
            if (players[i].timer > 1) --players[i].timer;
            masturbatingPlayers.push(i);

            let inHeavyMasturbation = players[i].updateHeavyMasturbation();
            if (inHeavyMasturbation) heavyMasturbatingPlayers.push(i);

            if (i == HUMAN_PLAYER) {
                masturbatingPlayers.push(i); // Double the chance of commenting on human player
                updateHumanPlayerMasturbationVisual();
            }
        }
    }

    if (heavyMasturbatingPlayers.length > 0) {
        masturbatingPlayers = heavyMasturbatingPlayers;
    }
    // Show a player masturbating while dealing or after the game, if there is one available
    if (masturbatingPlayers.length > 0
        && ((nextGamePhase == eGamePhase.DEAL && humanPlayer.out) || nextGamePhase == eGamePhase.EXCHANGE || nextGamePhase == eGamePhase.END_LOOP)) {
        var playerToShow = masturbatingPlayers[getRandomNumber(0, masturbatingPlayers.length)];
        var others_tags = [[players[playerToShow].getForfeitTrigger("masturbating"), OPPONENT_MASTURBATING]];
        if (players[playerToShow].forfeit[0] == PLAYER_HEAVY_MASTURBATING) {
            others_tags.unshift([players[playerToShow].getForfeitTrigger("heavy_masturbating"), OPPONENT_HEAVY_MASTURBATING]);
        }
        
        updateAllBehaviours(
            playerToShow,
            players[playerToShow].forfeit[0],
            others_tags
        );
    } else if (gamePhase == eGamePhase.DEAL && (ANIM_TIME > 0 || GAME_DELAY > 0)) {
        updateAllBehaviours(null, null, DEALING_CARDS);
    }
    
    return false;
}

/************************************************************
 * A player has 'finished' masturbating.
 ************************************************************/
function finishMasturbation (player) {
    // HARD SET STAGE
    players[player].stage += 1;
    players[player].finished = true;
    players[player].forfeit = [[[PLAYER_AFTER_MASTURBATING], [PLAYER_FINISHED_MASTURBATING]], CAN_SPEAK];
    players[player].stageChangeUpdate();
    
    /* update player dialogue */
    updateAllBehaviours(
        player, 
        PLAYER_FINISHED_MASTURBATING,
        [[players[player].getForfeitTrigger("finished_masturbating"), OPPONENT_FINISHED_MASTURBATING]]
    );
    players[player].ticksInStage = 0;
    players[player].timeInStage = 0;
    if (player == HUMAN_PLAYER) {
        updateHumanPlayerMasturbationVisual();
        $gameClimaxOverlay.removeClass('climax');
    }
    
    if (AUTO_FADE && globalSavedTableVisibility !== undefined) {
        forceTableVisibility(globalSavedTableVisibility);
        globalSavedTableVisibility = undefined;
    }
    allowProgression();
}
