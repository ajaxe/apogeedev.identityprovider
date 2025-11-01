(function () {
  $(function () {
    $('.multiple-uri-input').on('click', 'button', addUri);
    console.log({ count: $('.multiple-uri-input').length });
  });

  const addUri = function (e) {
    e.preventDefault();
    const current = $(e.delegateTarget);
    const v = current.find('input').val();
    console.log({ v });
    if (!v) return;

    const existing = current
      .siblings('.uri-display')
      .filter(
        (i, el) =>
          $(el).find('.uri-value').text().toUpperCase() === v.toUpperCase()
      );

    if (existing.length) return;

    const tmp = $('#uri-display-template').contents().clone();

    tmp.find('.uri-value').text(v);
    tmp.find('button').on('click', getDeleteHandler(tmp));

    current.before(tmp);
  };

  const getDeleteHandler = function (el) {
    return function (e) {
      el.remove();
      e.preventDefault();
    };
  };
})();
