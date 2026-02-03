$(function () {
  var page = $("body").data("page");

  // Shared behaviours
  $("#current-year").text(new Date().getFullYear());

  $(".nav-toggle").on("click", function () {
    $(".nav-links").toggleClass("is-open");
  });

  // Simple animated metrics on home
  if (page === "home") {
    $(".metric-value").each(function () {
      var $el = $(this);
      // Read the target value from data-target attribute (set from database)
      var target = parseFloat($el.data("target")) || 0;
      var current = 0;
      var steps = 30;
      var increment = target / steps;
      var interval = setInterval(function () {
        current += increment;
        if (current >= target) {
          current = target;
          clearInterval(interval);
        }
        $el.text(Math.round(current));
      }, 30);
    });
  }

  // Deadline countdown timer (used wherever the banner is rendered)
  var $deadlineCountdown = $("#deadline-countdown");
  if ($deadlineCountdown.length) {
    function updateDeadlineCountdown() {
      var now = new Date();
      var deadlineDateStr = $deadlineCountdown.data("deadline");
      var deadlineDate = null;

      if (deadlineDateStr) {
        var parts = String(deadlineDateStr).split("-");
        if (parts.length === 3) {
          var year = parseInt(parts[0], 10);
          var month = parseInt(parts[1], 10) - 1; // 0-indexed
          var day = parseInt(parts[2], 10);
          if (!isNaN(year) && !isNaN(month) && !isNaN(day)) {
            deadlineDate = new Date(year, month, day);
          }
        }
      }

      // Fallback to July 31st of current year if parsing fails
      if (!deadlineDate || isNaN(deadlineDate.getTime())) {
        var currentYear = now.getFullYear();
        deadlineDate = new Date(currentYear, 6, 31); // Month 6 = July (0-indexed)
      }

      // If deadline has passed, set for next year
      if (now > deadlineDate) {
        deadlineDate = new Date(
          deadlineDate.getFullYear() + 1,
          deadlineDate.getMonth(),
          deadlineDate.getDate()
        );
      }

      var diff = deadlineDate - now;
      var days = Math.floor(diff / (1000 * 60 * 60 * 24));
      var hours = Math.floor(
        (diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)
      );

      var countdownText = "";
      if (days > 0) {
        countdownText = days + " day" + (days > 1 ? "s" : "");
        if (days <= 7) {
          countdownText += " " + hours + " hour" + (hours !== 1 ? "s" : "");
        }
      } else {
        countdownText = hours + " hour" + (hours !== 1 ? "s" : "");
      }

      $deadlineCountdown.text(countdownText);
    }

    updateDeadlineCountdown();
    setInterval(updateDeadlineCountdown, 3600000); // Update every hour
  }

  // Home: services preview
  if (page === "home" || page === "services") {
    if (typeof TAX_SERVICES === "object" && TAX_SERVICES.length) {
      if (page === "home") {
        var homeServices = TAX_SERVICES.slice(0, 3);
        var $homeServicesContainer = $("#home-services");
        $.each(homeServices, function (_, service) {
          var card = [
            '<article class="card">',
            '<span class="badge">' + service.type + "</span>",
            "<h3>" + service.name + "</h3>",
            '<p class="card-meta">' + service.level + "</p>",
            "<p>" + service.description + "</p>",
            '<div class="card-footer">',
            '<span>' + service.highlight + "</span>",
            '<a href="contact.html" class="link-inline">Enquire</a>',
            "</div>",
            "</article>"
          ].join("");
          $homeServicesContainer.append(card);
        });
      }

      if (page === "services") {
        renderServices(TAX_SERVICES);
      }
    }
  }

  if (page === "home" || page === "blogs") {
    if (typeof BLOG_POSTS === "object" && BLOG_POSTS.length) {
      if (page === "home") {
        var $homeBlogs = $("#home-blogs");
        var homeBlogs = BLOG_POSTS.slice(0, 3);
        $.each(homeBlogs, function (_, post) {
          $homeBlogs.append(buildBlogCard(post));
        });
      }
      if (page === "blogs") {
        initBlogListing();
      }
    }
  }

  if (page === "blog-detail") {
    if (typeof BLOG_POSTS === "object" && BLOG_POSTS.length) {
      initBlogDetail();
    }
  }

  if (page === "service-detail") {
    if (typeof TAX_SERVICES === "object" && TAX_SERVICES.length) {
      initServiceDetail();
    }
  }

  if (page === "home" || page === "faqs") {
    if (typeof FAQ_ITEMS === "object" && FAQ_ITEMS.length) {
      if (page === "home") {
        var $homeFaqs = $("#home-faqs");
        var subset = FAQ_ITEMS.slice(0, 3);
        $homeFaqs.append(buildFaqAccordion(subset));
      }
      if (page === "faqs") {
        initFaqListing();
      }
    }
  }

  if (page === "privacy" || page === "terms") {
    initLegalPage(page);
  }

  // Forms - Client-side validation only, allow server-side submission
  $("#mini-enquiry").on("submit", function (e) {
    if (!this.checkValidity()) {
    e.preventDefault();
      return false;
    }
    // Allow form to submit to server
  });

  // Auto-hide success/error messages in mini enquiry form after 5 seconds
  if ($(".mini-enquiry-message").length) {
    // Check if it's a success message (green background)
    var $message = $(".mini-enquiry-message");
    var bgColor = $message.css("background-color");
    
    // Reset form if success message is shown
    if (bgColor === "rgb(39, 180, 106)" || bgColor === "#27b46a") {
      $("#mini-enquiry")[0].reset();
    }
    
    // Auto-hide after 5 seconds
    setTimeout(function() {
      $message.fadeOut(300, function() {
        $(this).remove();
      });
    }, 5000);
  }

  $("#contact-form").on("submit", function (e) {
    var $status = $("#contact-status");
    $status.removeClass("success error");
    if (!this.checkValidity()) {
      e.preventDefault();
      $status.addClass("error").text("Please fill all required fields correctly.");
      return false;
    }
    // Allow form to submit to server - success message will come from TempData
  });

  $("#service-contact-form").on("submit", function (e) {
    var $status = $("#service-contact-status");
    $status.removeClass("success error");
    if (!this.checkValidity()) {
      e.preventDefault();
      $status.addClass("error").text("Please fill all required fields correctly.");
      return false;
    }
    // Allow form to submit to server - success message will come from TempData
  });


  // Accordion toggles (event delegation)
  $(document).on("click", ".accordion-header", function () {
    var $item = $(this).closest(".accordion-item");
    var $accordionList = $item.closest(".accordion-list-modern, .accordion-list");
    var isCurrentlyOpen = $item.hasClass("is-open");
    
    // Close all items in the same accordion list
    $accordionList.find(".accordion-item").removeClass("is-open");
    
    // If the clicked item was not open, open it now
    if (!isCurrentlyOpen) {
      $item.addClass("is-open");
    }
  });

  // Testimonials slider
  if ($(".testimonials-slider").length) {
    var $slider = $(".testimonials-slider");
    var $track = $(".testimonials-track");
    var $cards = $(".testimonial-card", $track);
    var $prevBtn = $(".slider-arrow-left");
    var $nextBtn = $(".slider-arrow-right");
    var currentIndex = 0;
    var cardsPerView = 3;
    var totalCards = $cards.length;

    function getCardsPerView() {
      var width = $(window).width();
      if (width <= 720) {
        return 1;
      } else if (width <= 900) {
        return 2;
      } else {
        return 3;
      }
    }

    function updateSlider() {
      cardsPerView = getCardsPerView();
      var maxIndex = Math.max(0, totalCards - cardsPerView);
      currentIndex = Math.min(currentIndex, maxIndex);
      currentIndex = Math.max(0, currentIndex);

      if (totalCards <= cardsPerView) {
        $prevBtn.prop("disabled", true);
        $nextBtn.prop("disabled", true);
        $track.css("transform", "translateX(0)");
        return;
      }

      var cardWidth = $cards.first().outerWidth(true);
      var translateX = -(currentIndex * cardWidth);
      $track.css("transform", "translateX(" + translateX + "px)");

      $prevBtn.prop("disabled", currentIndex === 0);
      $nextBtn.prop("disabled", currentIndex >= maxIndex);
    }

    $prevBtn.on("click", function () {
      if (currentIndex > 0) {
        currentIndex--;
        updateSlider();
      }
    });

    $nextBtn.on("click", function () {
      cardsPerView = getCardsPerView();
      var maxIndex = Math.max(0, totalCards - cardsPerView);
      if (currentIndex < maxIndex) {
        currentIndex++;
        updateSlider();
      }
    });

    $(window).on("resize", function () {
      updateSlider();
    });

    // Initialize on load
    setTimeout(function() {
      updateSlider();
    }, 100);
  }

  // Team slider
  if ($(".team-slider").length) {
    var $teamSlider = $(".team-slider");
    var $teamTrack = $(".team-track");
    var $teamCards = $(".team-card", $teamTrack);
    var $teamPrevBtn = $(".team-slider-arrow-left");
    var $teamNextBtn = $(".team-slider-arrow-right");
    var teamCurrentIndex = 0;
    var teamCardsPerView = 3;
    var teamTotalCards = $teamCards.length;

    function getTeamCardsPerView() {
      var width = $(window).width();
      if (width <= 720) {
        return 1;
      } else if (width <= 900) {
        return 2;
      } else {
        return 3;
      }
    }

    function updateTeamSlider() {
      teamCardsPerView = getTeamCardsPerView();
      var maxIndex = Math.max(0, teamTotalCards - teamCardsPerView);
      teamCurrentIndex = Math.min(teamCurrentIndex, maxIndex);
      teamCurrentIndex = Math.max(0, teamCurrentIndex);

      if (teamTotalCards <= teamCardsPerView) {
        $teamPrevBtn.prop("disabled", true);
        $teamNextBtn.prop("disabled", true);
        $teamTrack.css("transform", "translateX(0)");
        return;
      }

      var cardWidth = $teamCards.first().outerWidth(true);
      var translateX = -(teamCurrentIndex * cardWidth);
      $teamTrack.css("transform", "translateX(" + translateX + "px)");

      $teamPrevBtn.prop("disabled", teamCurrentIndex === 0);
      $teamNextBtn.prop("disabled", teamCurrentIndex >= maxIndex);
    }

    $teamPrevBtn.on("click", function () {
      if (teamCurrentIndex > 0) {
        teamCurrentIndex--;
        updateTeamSlider();
      }
    });

    $teamNextBtn.on("click", function () {
      teamCardsPerView = getTeamCardsPerView();
      var maxIndex = Math.max(0, teamTotalCards - teamCardsPerView);
      if (teamCurrentIndex < maxIndex) {
        teamCurrentIndex++;
        updateTeamSlider();
      }
    });

    $(window).on("resize", function () {
      updateTeamSlider();
    });

    // Initialize on load
    setTimeout(function() {
      updateTeamSlider();
    }, 100);
  }

  function renderServices(services) {
    var $list = $("#service-list");
    var $filters = $("#service-filters");
    $list.empty();
    $filters.empty();

    var types = ["All"];
    $.each(services, function (_, s) {
      if (types.indexOf(s.type) === -1) {
        types.push(s.type);
      }
    });

    $.each(types, function (_, type) {
      var pill = $('<button type="button" class="filter-pill"></button>').text(type);
      if (type === "All") {
        pill.addClass("is-active");
      }
      pill.on("click", function () {
        $(".filter-pill", $filters).removeClass("is-active");
        $(this).addClass("is-active");
        var filtered = type === "All" ? services : $.grep(services, function (s) { return s.type === type; });
        renderServicesList(filtered);
      });
      $filters.append(pill);
    });

    function renderServicesList(list) {
      $list.empty();
      $.each(list, function (_, s) {
        var card = [
          '<article class="card" id="' + (s.id || '') + '">',
          '<span class="badge badge-outline">' + s.type + "</span>",
          "<h3>" + s.name + "</h3>",
          '<p class="card-meta">' + s.level + "</p>",
          "<p>" + s.description + "</p>",
          '<div class="card-footer">',
          '<span>' + s.highlight + "</span>",
          '<a href="service-detail.html?id=' + (s.id || '') + '" class="link-inline">Learn More</a>',
          "</div>",
          "</article>"
        ].join("");
        $list.append(card);
      });
    }

    renderServicesList(services);
  }

  function buildBlogCard(post) {
    var tags = post.tags && post.tags.length ? post.tags.join(" · ") : "";
    var blogUrl = post.url || ("blog-detail.html?id=" + post.id);
    return [
      '<article class="card">',
      '<div class="card-meta">' + post.category + (tags ? " · " + tags : "") + "</div>",
      "<h3>" + post.title + "</h3>",
      "<p>" + post.excerpt + "</p>",
      '<div class="card-footer">',
      "<span>" + post.readTime + "</span>",
      '<a href="' + blogUrl + '" class="link-inline">Read</a>',
      "</div>",
      "</article>"
    ].join("");
  }

  function initBlogListing() {
    var $list = $("#blog-list");
    var $filters = $("#blog-category-filters");

    var categories = ["All"];
    $.each(BLOG_POSTS, function (_, p) {
      if (categories.indexOf(p.category) === -1) {
        categories.push(p.category);
      }
    });

    $.each(categories, function (_, cat) {
      var pill = $('<button type="button" class="filter-pill"></button>').text(cat);
      if (cat === "All") {
        pill.addClass("is-active");
      }
      pill.on("click", function () {
        $(".filter-pill", $filters).removeClass("is-active");
        $(this).addClass("is-active");
        renderBlogs();
      });
      $filters.append(pill);
    });

    function renderBlogs() {
      var activeCategory = $(".filter-pill.is-active", $filters).text();
      var $emptyState = $("#blog-empty-state");
      $list.empty();

      var filtered = $.grep(BLOG_POSTS, function (p) {
        var matchesCategory = activeCategory === "All" || p.category === activeCategory;
        return matchesCategory;
      });

      if (filtered.length > 0) {
        $emptyState.hide();
        $.each(filtered, function (_, p) {
          $list.append(buildBlogCard(p));
        });
      } else {
        $emptyState.show();
      }
    }

    renderBlogs();
  }

  function initBlogDetail() {
    var $container = $("#blog-detail-container");
    var $error = $("#blog-detail-error");
    
    // Get blog ID from URL parameter
    var urlParams = new URLSearchParams(window.location.search);
    var blogId = urlParams.get("id");
    
    if (!blogId) {
      $container.hide();
      $error.show();
      return;
    }
    
    // Find the blog post
    var blogPost = null;
    $.each(BLOG_POSTS, function (_, post) {
      if (post.id === blogId) {
        blogPost = post;
        return false; // break
      }
    });
    
    if (!blogPost) {
      $container.hide();
      $error.show();
      return;
    }
    
    // Build blog detail HTML
    var tagsHtml = "";
    if (blogPost.tags && blogPost.tags.length) {
      var tagItems = [];
      $.each(blogPost.tags, function (_, tag) {
        tagItems.push('<span class="blog-tag">' + tag + "</span>");
      });
      tagsHtml = '<div class="blog-detail-tags">' + tagItems.join("") + "</div>";
    }
    
    var blogDetailHtml = [
      '<div class="blog-detail-header">',
      '<a href="blogs.html" class="blog-back-link">← Back to Blogs</a>',
      '<div class="blog-detail-meta">',
      '<span class="blog-detail-category">' + blogPost.category + "</span>",
      '<span class="blog-detail-read-time">' + blogPost.readTime + "</span>",
      "</div>",
      "<h1>" + blogPost.title + "</h1>",
      tagsHtml,
      "</div>",
      '<div class="blog-detail-content">' + (blogPost.content || blogPost.excerpt) + "</div>",
      '<div class="blog-detail-footer">',
      '<a href="blogs.html" class="btn btn-secondary">View All Blogs</a>',
      '<a href="contact.html" class="btn btn-primary">Get Expert Help</a>',
      "</div>"
    ].join("");
    
    $container.html(blogDetailHtml);
    $error.hide();
    
    // Update page title
    document.title = blogPost.title + " | TaxHelperToday";
  }

  function initServiceDetail() {
    var $container = $("#service-detail-container");
    var $error = $("#service-detail-error");
    
    // Get service ID from URL parameter
    var urlParams = new URLSearchParams(window.location.search);
    var serviceId = urlParams.get("id");
    
    if (!serviceId) {
      $container.hide();
      $error.show();
      return;
    }
    
    // Find the service
    var service = null;
    $.each(TAX_SERVICES, function (_, s) {
      if (s.id === serviceId) {
        service = s;
        return false; // break
      }
    });
    
    if (!service) {
      $container.hide();
      $error.show();
      return;
    }
    
    // Build service detail HTML
    var serviceContent = service.content || "<p>" + service.description + "</p>";
    var serviceDetailHtml = [
      '<div class="service-detail-header">',
      '<div class="service-detail-meta">',
      '<span class="service-detail-type">' + service.type + "</span>",
      '<span class="service-detail-level">' + service.level + "</span>",
      "</div>",
      "<h1>" + service.name + "</h1>",
      '<p class="service-detail-highlight">' + service.highlight + "</p>",
      "</div>",
      '<div class="service-detail-content">',
      serviceContent,
      "</div>"
    ].join("");
    
    $container.html(serviceDetailHtml);
    $error.hide();
    
    // Show contact form and pre-fill service name
    $("#service-contact-section").show();
    $("#service-contact-service").val(service.name);
    
    // Update page title
    document.title = service.name + " | TaxHelperToday";
  }

  function buildFaqAccordion(items) {
    var html = "";
    $.each(items, function (_, item) {
      html += [
        '<div class="accordion-item">',
        '<div class="accordion-header">',
        "<h3>" + item.question + "</h3>",
        '<span class="accordion-toggle"><span class="toggle-icon">+</span></span>',
        "</div>",
        '<div class="accordion-body"><p>' + item.answer + "</p></div>",
        "</div>"
      ].join("");
    });
    return html;
  }

  function initFaqListing() {
    var $tabs = $("#faq-tabs");

    // Map category names to tab IDs
    function getTabId(category) {
      var map = {
        "All": "all",
        "ITR Filing": "itr",
        "Trust & Safety": "trust",
        "Process": "process",
        "Support": "support"
      };
      return map[category] || category.toLowerCase().replace(/\s+/g, "-").replace(/&/g, "").replace(/amp;/g, "");
    }

    var categories = ["All"];
    $.each(FAQ_ITEMS, function (_, item) {
      if (categories.indexOf(item.category) === -1) {
        categories.push(item.category);
      }
    });

    // Create tabs
    $.each(categories, function (index, cat) {
      var tabId = getTabId(cat);
      var tab = $('<button type="button" class="faq-tab" role="tab" aria-selected="false" data-category="' + cat + '" data-tab-id="' + tabId + '"></button>').text(cat);
      if (cat === "All") {
        tab.addClass("is-active").attr("aria-selected", "true");
      }
      tab.on("click", function () {
        switchTab(cat, tabId);
      });
      $tabs.append(tab);
    });

    function switchTab(category, tabId) {
      // Update tab states
      $(".faq-tab").removeClass("is-active").attr("aria-selected", "false");
      $('.faq-tab[data-category="' + category + '"]').addClass("is-active").attr("aria-selected", "true");
      
      // Hide all panels
      $(".faq-tab-panel").hide();
      
      // Show active panel
      var $panel = $("#faq-tab-" + tabId);
      if ($panel.length) {
        $panel.show();
      }
      
      // Render FAQs for active tab
      renderTab(category, tabId);
    }

    function renderTab(category, tabId) {
      var $panel = $("#faq-tab-" + tabId);
      if (!$panel.length) {
        // Create panel if it doesn't exist
        var $wrapper = $(".faq-tab-content-wrapper");
        $panel = $('<div id="faq-tab-' + tabId + '" class="faq-tab-panel" role="tabpanel" style="display: none;"></div>');
        var $list = $('<div id="faq-list-' + tabId + '" class="accordion-list-modern"></div>');
        $panel.append($list);
        $wrapper.append($panel);
      }

      var $list = $("#faq-list-" + tabId);
      if (!$list.length) {
        $list = $('<div id="faq-list-' + tabId + '" class="accordion-list-modern"></div>');
        $panel.empty().append($list);
      }

      var filtered = $.grep(FAQ_ITEMS, function (item) {
        return category === "All" || item.category === category;
      });

      $list.empty();
      if (filtered.length > 0) {
        $list.append(buildFaqAccordion(filtered));
      } else {
        $list.append('<div class="faq-empty-state"><span class="empty-icon">📋</span><h3>No FAQs in this category</h3><p>Check other categories for more information.</p></div>');
      }
    }

    function renderAllTabs() {
      $.each(categories, function (_, cat) {
        var tabId = getTabId(cat);
        renderTab(cat, tabId);
      });
    }

    // Initial render
    renderAllTabs();
    switchTab("All", "all");
  }

  function initLegalPage(page) {
    var key = page === "privacy" ? "privacy" : "terms";
    var blocks = (LEGAL_CONTENT && LEGAL_CONTENT[key]) || [];
    var $container = key === "privacy" ? $("#privacy-content") : $("#terms-content");
    
    // Only populate if container is empty or only contains comments
    var hasContent = $container.html().trim().replace(/<!--[\s\S]*?-->/g, '').trim().length > 0;
    if (hasContent) {
      return; // Skip initialization if static content already exists
    }
    
    $container.empty();
    $.each(blocks, function (_, block) {
      $container.append("<h2>" + block.heading + "</h2>");
      $container.append("<p>" + block.body + "</p>");
    });
  }
});

