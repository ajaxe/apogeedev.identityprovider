(function () {
  $(function () {
    $('.multiple-uri-input').on('click', 'button', addUri);
    $('.uri-display').on('click', 'button', (e) =>
      getDeleteHandler($(e.delegateTarget))(e)
    );
  });

  const addUri = function (e) {
    e.preventDefault();
    const current = $(e.delegateTarget);
    const input = current.find('input');
    const v = input.val();

    if (!URL.canParse(v)) return;

    const url = new URL(v);
    if (
      !checkValidity({
        input: input[0],
        message: 'Must be a valid URI',
        check: () => url.protocol === 'https:',
      })
    ) {
      return;
    }

    const existing = current
      .siblings('.uri-display')
      .filter(
        (i, el) =>
          $(el).find('.uri-value').text().toUpperCase() === v.toUpperCase()
      );

    if (
      !checkValidity({
        input: input[0],
        message: 'URI already added.',
        check: () => existing.length === 0,
      })
    ) {
      return;
    }

    const tmp = $('#uri-display-template').contents().clone();

    tmp.find('.uri-value').text(v);
    tmp.find('button').on('click', getDeleteHandler(tmp));

    current.before(tmp);
    input.val('');
  };

  const getDeleteHandler = function (el) {
    return function (e) {
      el.remove();
      e.preventDefault();
    };
  };

  const checkValidity = function ({ input, message, check }) {
    const valid = check();
    if (valid) {
      input.setCustomValidity('');
    } else {
      input.setCustomValidity(message);
      input.reportValidity();
    }
    return valid;
  };
})();
