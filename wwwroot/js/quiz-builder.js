document.addEventListener("DOMContentLoaded", function () {
  const questionsContainer = document.getElementById("questions-container");

  function updateNamesAndIds() {
    const questionBlocks =
      questionsContainer.querySelectorAll(".question-block");
    questionBlocks.forEach((questionBlock, questionIndex) => {
      questionBlock
        .querySelectorAll('input[name^="Text"], input[name^="Points"]')
        .forEach((input) => {
          const name = input.name.split(".").pop();
          input.name = `Questions[${questionIndex}].${name}`;
        });

      const answerGroups = questionBlock.querySelectorAll(".answer-group");
      const questionRadioGroupName = `Questions[${questionIndex}].CorrectAnswerIndex`;

      answerGroups.forEach((answerGroup, answerIndex) => {
        answerGroup.querySelectorAll("input, select").forEach((input) => {
          const name = input.name.split(".").pop();
          if (input.type === "radio") {
            input.name = `Questions[${questionIndex}].CorrectAnswerIndex`;
            input.value = answerIndex;
          } else {
            input.name = `Questions[${questionIndex}].Answers[${answerIndex}].${name}`;
          }
        });
      });
    });
  }

  document
    .getElementById("add-question")
    .addEventListener("click", function () {
      const questionTemplate =
        document.getElementById("question-template").innerHTML;
      const newQuestionDiv = document.createElement("div");
      newQuestionDiv.innerHTML = questionTemplate;
      questionsContainer.appendChild(newQuestionDiv.firstElementChild);
      updateNamesAndIds();
    });

  questionsContainer.addEventListener("click", function (e) {
    if (e.target.classList.contains("remove-question")) {
      e.target.closest(".question-block").remove();
      updateNamesAndIds();
    }

    if (e.target.classList.contains("add-answer")) {
      const answersContainer =
        e.target.parentElement.querySelector(".answers-container");
      const lastAnswerGroup = answersContainer.querySelector(
        ".answer-group:last-of-type",
      );
      if (lastAnswerGroup) {
        const newAnswerGroup = lastAnswerGroup.cloneNode(true);
        newAnswerGroup
          .querySelectorAll("input")
          .forEach((input) => (input.value = ""));
        newAnswerGroup.querySelector('input[type="radio"]').checked = false;
        answersContainer.appendChild(newAnswerGroup);
        updateNamesAndIds();
      }
    }

    if (e.target.classList.contains("remove-answer")) {
      const answerGroup = e.target.closest(".answer-group");
      if (
        answerGroup.parentElement.querySelectorAll(".answer-group").length > 2
      ) {
        answerGroup.remove();
        updateNamesAndIds();
      }
    }
  });

  updateNamesAndIds();
});
