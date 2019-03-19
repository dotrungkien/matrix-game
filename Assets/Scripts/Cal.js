calculate_score_and_draw_lines: function () {
    var player_scores = [0, 0, 0, 0];
    for (var i = 0; i < game.number_of_players; i++) {
        for (var j = 2; j <= 10; j++) {
            for (var k = 2; k <= 10; k++) {
                // lr
                if (game.same_values(game.state[i][j][k - 2], game.state[i][j][k - 1], game.state[i][j][k]) || game.same_values(game.state[i][j][k + 1], game.state[i][j][k - 1], game.state[i][j][k]) || game.same_values(game.state[i][j][k + 2], game.state[i][j][k + 1], game.state[i][j][k])) {
                    player_scores[i] += game.state[i][j][k];
                    var target = window["line_" + (i + 1) + "_" + (j - 2) + "_" + (k - 2) + "_" + "lr"];
                    if (target.opacity() == 0) {
                        var tween = new Konva.Tween({
                            node: target,
                            duration: 0.5,
                            easing: Konva.Easings.ElasticEaseOut,
                            opacity: 1,
                            strokeWidth: 2
                        });
                        tween.play();
                    }
                }
                // tb
                if (game.same_values(game.state[i][j - 1][k], game.state[i][j - 2][k], game.state[i][j][k]) || game.same_values(game.state[i][j - 1][k], game.state[i][j + 1][k], game.state[i][j][k]) || game.same_values(game.state[i][j + 2][k], game.state[i][j + 1][k], game.state[i][j][k])) {
                    player_scores[i] += game.state[i][j][k] == 0 ? 10 : game.state[i][j][k];
                    var target = window["line_" + (i + 1) + "_" + (j - 2) + "_" + (k - 2) + "_" + "tb"];
                    if (target.opacity() == 0) {
                        var tween = new Konva.Tween({
                            node: target,
                            duration: 0.5,
                            easing: Konva.Easings.ElasticEaseOut,
                            opacity: 1,
                            strokeWidth: 2
                        });
                        tween.play();
                    }
                }
                // tlbr
                if (game.same_values(game.state[i][j - 1][k - 1], game.state[i][j - 2][k - 2], game.state[i][j][k]) || game.same_values(game.state[i][j - 1][k - 1], game.state[i][j + 1][k + 1], game.state[i][j][k]) || game.same_values(game.state[i][j + 2][k + 2], game.state[i][j + 1][k + 1], game.state[i][j][k])) {
                    player_scores[i] += game.state[i][j][k] == 0 ? 10 : game.state[i][j][k];
                    var target = window["line_" + (i + 1) + "_" + (j - 2) + "_" + (k - 2) + "_" + "tlbr"];
                    if (target.opacity() == 0) {
                        var tween = new Konva.Tween({
                            node: target,
                            duration: 0.5,
                            easing: Konva.Easings.ElasticEaseOut,
                            opacity: 1,
                            strokeWidth: 2
                        });
                        tween.play();
                    }
                }
                // trbl
                if (game.same_values(game.state[i][j - 1][k + 1], game.state[i][j - 2][k + 2], game.state[i][j][k]) || game.same_values(game.state[i][j - 1][k + 1], game.state[i][j + 1][k - 1], game.state[i][j][k]) || game.same_values(game.state[i][j + 2][k - 2], game.state[i][j + 1][k - 1], game.state[i][j][k])) {
                    player_scores[i] += game.state[i][j][k] == 0 ? 10 : game.state[i][j][k];
                    var target = window["line_" + (i + 1) + "_" + (j - 2) + "_" + (k - 2) + "_" + "trbl"];
                    if (target.opacity() == 0) {
                        var tween = new Konva.Tween({
                            node: target,
                            duration: 0.5,
                            easing: Konva.Easings.ElasticEaseOut,
                            opacity: 1,
                            strokeWidth: 2
                        });
                        tween.play();
                    }
                }
            }
        }
    }
    game.set_scores(player_scores);
},
same_values: function (a, b, c) {
    return (a == b && b == c && a != -1);
},