using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;
using TaxHelperToday.Modules.Content.Domain.Entities;

namespace TaxHelperToday.Infrastructure.Services;

public class DataSeedingService
{
    private readonly ApplicationDbContext _context;
    private readonly IBlogService _blogService;
    private readonly IServiceService _serviceService;
    private readonly IFaqService _faqService;
    private readonly IPageService _pageService;
    private readonly ILogger<DataSeedingService> _logger;

    public DataSeedingService(
        ApplicationDbContext context,
        IBlogService blogService,
        IServiceService serviceService,
        IFaqService faqService,
        IPageService pageService,
        ILogger<DataSeedingService> logger)
    {
        _context = context;
        _blogService = blogService;
        _serviceService = serviceService;
        _faqService = faqService;
        _pageService = pageService;
        _logger = logger;
    }

    public async Task SeedDataAsync(long adminUserId)
    {
        _logger.LogInformation("Starting data seeding...");

        try
        {
            // Seed Services
            await SeedServicesAsync();

            // Seed Blog Posts
            await SeedBlogPostsAsync(adminUserId);

            // Seed FAQs
            await SeedFaqsAsync();

            // Seed Pages
            await SeedPagesAsync();

            _logger.LogInformation("Data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during data seeding");
            throw;
        }
    }

    private async Task SeedServicesAsync()
    {
        var existingServices = await _context.Services.CountAsync();
        if (existingServices > 0)
        {
            _logger.LogInformation("Services already exist, skipping seed");
            return;
        }

        var services = new[]
        {
            new { id = "itr-individual", name = "Individual ITR Filing", type = "Individuals", level = "Semi Dynamic", description = "End-to-end assisted filing for salaried employees, pensioners, and first-time filers with guidance on deductions and proofs.", highlight = "Ideal for salaried and first-time investors.", content = "<p>Our Individual ITR Filing service is designed to make tax filing simple and stress-free for salaried employees, pensioners, and first-time filers. We understand that tax filing can be overwhelming, especially if you're doing it for the first time or have multiple income sources.</p><h2>What's Included</h2><ul><li><strong>Complete ITR Preparation:</strong> Our CA experts review all your documents including Form 16, bank statements, investment proofs, and other income sources</li><li><strong>Deduction Optimization:</strong> We identify all eligible deductions under Sections 80C, 80D, 24(b), and others to maximize your tax savings</li><li><strong>Documentation Guidance:</strong> Clear checklist of documents needed and help organizing them properly</li><li><strong>E-filing Support:</strong> We file your return electronically and help with e-verification</li><li><strong>Refund Tracking:</strong> Assistance in tracking your refund status and ensuring timely processing</li><li><strong>Notice Support:</strong> Help with any notices or queries from the Income Tax Department</li></ul><h2>Who Should Use This Service</h2><p>This service is perfect for:</p><ul><li>Salaried employees with income from salary and investments</li><li>Pensioners receiving pension income</li><li>First-time tax filers who need guidance</li><li>Individuals with income from house property, capital gains, or other sources</li><li>Anyone who wants expert assistance to ensure accurate filing</li></ul><h2>Process</h2><p>Once you sign up, our team will guide you through a simple process:</p><ol><li>Share your documents (Form 16, investment proofs, bank statements)</li><li>Our CA reviews and prepares your return</li><li>You review the draft return</li><li>We file it electronically on your behalf</li><li>We help with e-verification and refund tracking</li></ol>" },
            new { id = "itr-business", name = "Business & Professional ITR", type = "Business", level = "Dynamic", description = "Comprehensive ITR filing for proprietorships, partners, and professionals with books of account review and compliance checklists.", highlight = "Suited for consultants, doctors, and freelancers.", content = "<p>Business and professional ITR filing requires careful attention to books of accounts, compliance requirements, and proper documentation. Our service ensures your business ITR is filed accurately while maximizing legitimate deductions.</p><h2>What's Included</h2><ul><li><strong>Books of Account Review:</strong> Comprehensive review of your financial records, ledgers, and supporting documents</li><li><strong>Income Computation:</strong> Accurate calculation of business income, professional fees, and other revenue streams</li><li><strong>Expense Verification:</strong> Review of business expenses, depreciation, and other deductions</li><li><strong>Compliance Checklist:</strong> Ensure all regulatory requirements are met before filing</li><li><strong>ITR-3/ITR-4 Selection:</strong> Help choosing the correct ITR form based on your business structure</li><li><strong>Presumptive Taxation:</strong> Guidance on Section 44AD, 44ADA, and 44AE if applicable</li><li><strong>Audit Support:</strong> Assistance if tax audit is required under Section 44AB</li></ul><h2>Who Should Use This Service</h2><p>This service is ideal for:</p><ul><li>Proprietorships and sole proprietors</li><li>Partnership firms</li><li>Professionals (doctors, lawyers, consultants, architects)</li><li>Freelancers and independent contractors</li><li>Small business owners with turnover up to ₹2 crores</li></ul><h2>Key Benefits</h2><ul><li>Expert review by qualified CAs</li><li>Maximize legitimate business deductions</li><li>Ensure compliance with all tax laws</li><li>Reduce risk of notices and penalties</li><li>Year-round support for tax queries</li></ul>" },
            new { id = "gst-compliance", name = "GST Registration & Returns", type = "GST", level = "Dynamic", description = "Registration, monthly/quarterly returns, and reconciliation to keep your GST profile healthy and notice-ready.", highlight = "From registration to ongoing support.", content = "<p>GST compliance is critical for businesses operating in India. Our comprehensive GST service covers everything from registration to monthly/quarterly return filing, ensuring you stay compliant and avoid penalties.</p><h2>What's Included</h2><ul><li><strong>GST Registration:</strong> Assistance with new GST registration, including document preparation and application filing</li><li><strong>Monthly/Quarterly Returns:</strong> Timely filing of GSTR-1, GSTR-3B, and other applicable returns</li><li><strong>Input Tax Credit Reconciliation:</strong> Matching of ITC with GSTR-2A/2B to maximize credits and identify discrepancies</li><li><strong>Annual Return (GSTR-9):</strong> Preparation and filing of annual GST return</li><li><strong>E-Invoicing Support:</strong> Guidance on e-invoicing requirements if applicable</li><li><strong>Notice Management:</strong> Help responding to GST notices and queries</li><li><strong>Compliance Monitoring:</strong> Regular reminders for filing deadlines and compliance requirements</li></ul><h2>Who Should Use This Service</h2><p>This service is essential for:</p><ul><li>Businesses with annual turnover above ₹20 lakh (₹10 lakh for special states)</li><li>E-commerce sellers and operators</li><li>Businesses requiring regular GST compliance</li><li>Companies looking to streamline their GST processes</li><li>Businesses that want to avoid penalties and notices</li></ul><h2>Key Benefits</h2><ul><li>Timely filing to avoid late fees and penalties</li><li>Maximize Input Tax Credit (ITC) claims</li><li>Reduce risk of GST notices</li><li>Expert guidance on complex GST provisions</li><li>Ongoing support throughout the year</li></ul>" },
            new { id = "tds-compliance", name = "TDS & TCS Compliance", type = "TDS", level = "Dynamic", description = "Management of TDS deductions, challans, and quarterly returns for businesses and property transactions.", highlight = "Includes guidance on Sections 194-IA, 194J and more.", content = "<p>Proper TDS (Tax Deducted at Source) and TCS (Tax Collected at Source) compliance is crucial for businesses and individuals making certain payments. Our service ensures accurate TDS deductions, timely deposit, and proper return filing.</p><h2>What's Included</h2><ul><li><strong>TDS Calculation:</strong> Accurate calculation of TDS on various payments (salary, rent, professional fees, interest, etc.)</li><li><strong>Challan Management:</strong> Preparation and filing of TDS challans (Form 26QB for property, Form 26QC for rent)</li><li><strong>Quarterly Returns:</strong> Filing of TDS returns (Form 24Q, 26Q, 27Q, 27EQ) and TCS returns</li><li><strong>Section-wise Guidance:</strong> Expert advice on applicable TDS sections (194-IA, 194J, 194A, 194H, etc.)</li><li><strong>Certificate Issuance:</strong> Help with issuing Form 16, Form 16A, and Form 27D</li><li><strong>Reconciliation:</strong> Matching TDS credits with Form 26AS</li><li><strong>Notice Support:</strong> Assistance with TDS-related notices and queries</li></ul><h2>Who Should Use This Service</h2><p>This service is important for:</p><ul><li>Businesses making payments subject to TDS</li><li>Property buyers (Section 194-IA compliance)</li><li>Individuals paying rent above ₹40,000/month (Section 194-IB)</li><li>Companies with salary payments</li><li>Anyone required to file TDS returns</li></ul><h2>Key Benefits</h2><ul><li>Ensure compliance with TDS provisions</li><li>Avoid penalties for non-deduction or late deposit</li><li>Proper documentation and record-keeping</li><li>Timely return filing to avoid interest and penalties</li><li>Expert guidance on complex TDS scenarios</li></ul>" },
            new { id = "notice-management", name = "Notice Response & Representation", type = "Support", level = "Dynamic", description = "Structured response strategy for income tax and GST notices with documentation, replies, and representation support.", highlight = "Designed to reduce anxiety around notices.", content = "<p>Receiving a tax notice can be stressful, but most notices are routine queries that can be resolved with proper documentation and timely response. Our notice management service helps you understand, respond to, and resolve tax notices efficiently.</p><h2>What's Included</h2><ul><li><strong>Notice Analysis:</strong> Detailed review of the notice to understand the issue raised by the department</li><li><strong>Document Gathering:</strong> Help identifying and organizing all relevant documents needed for response</li><li><strong>Response Drafting:</strong> Professional drafting of replies with proper legal language and supporting evidence</li><li><strong>Representation:</strong> CA representation before tax authorities if required</li><li><strong>Follow-up:</strong> Tracking of notice status and follow-up with department</li><li><strong>Appeal Support:</strong> Assistance with appeals if the issue cannot be resolved at the initial stage</li><li><strong>Preventive Guidance:</strong> Advice on avoiding future notices through better compliance</li></ul><h2>Types of Notices We Handle</h2><ul><li>Section 143(1) - Intimation notices</li><li>Section 142(1) - Scrutiny notices</li><li>Section 148 - Reassessment notices</li><li>Section 139(9) - Defective return notices</li><li>GST mismatch notices</li><li>TDS/TCS related notices</li><li>Demand notices and penalty orders</li></ul><h2>Who Should Use This Service</h2><p>This service is essential for:</p><ul><li>Anyone who has received a tax notice</li><li>Businesses facing GST notices</li><li>Individuals with mismatch or scrutiny notices</li><li>Taxpayers who want professional representation</li><li>Anyone unsure how to respond to a notice</li></ul><h2>Key Benefits</h2><ul><li>Reduce stress and anxiety around notices</li><li>Professional handling increases chances of favorable resolution</li><li>Timely response to avoid additional penalties</li><li>Expert knowledge of tax laws and procedures</li><li>Peace of mind knowing your case is handled properly</li></ul>" },
            new { id = "tax-planning", name = "Year-Round Tax Planning", type = "Planning", level = "Semi Dynamic", description = "Personalised plan for investments, deductions, and advance tax so that filings become a formality.", highlight = "Ideal for those who want to avoid last-minute rush.", content = "<p>Tax planning is not just about filing returns—it's about making smart financial decisions throughout the year to minimize your tax liability legally. Our year-round tax planning service helps you plan ahead and avoid the last-minute rush.</p><h2>What's Included</h2><ul><li><strong>Tax Liability Assessment:</strong> Early calculation of your estimated tax liability for the year</li><li><strong>Investment Planning:</strong> Personalized recommendations for tax-saving investments (ELSS, PPF, NSC, etc.)</li><li><strong>Deduction Optimization:</strong> Strategies to maximize deductions under various sections (80C, 80D, 24(b), etc.)</li><li><strong>Advance Tax Planning:</strong> Guidance on advance tax payments to avoid interest under Section 234B and 234C</li><li><strong>Quarterly Reviews:</strong> Regular check-ins to track your tax-saving progress</li><li><strong>Regime Selection:</strong> Help choosing between old and new tax regimes based on your situation</li><li><strong>Year-End Planning:</strong> Last-minute tax-saving opportunities before March 31st</li></ul><h2>Who Should Use This Service</h2><p>This service is perfect for:</p><ul><li>High-income earners who want to optimize tax savings</li><li>Business owners and professionals</li><li>Individuals with multiple income sources</li><li>Anyone who wants to plan taxes proactively</li><li>People who want to avoid last-minute tax planning stress</li></ul><h2>Key Benefits</h2><ul><li>Maximize tax savings through proper planning</li><li>Avoid last-minute rush and poor investment decisions</li><li>Ensure advance tax compliance</li><li>Better financial planning throughout the year</li><li>Peace of mind knowing your taxes are planned</li></ul><h2>Planning Timeline</h2><p>We work with you throughout the financial year:</p><ul><li><strong>April-June:</strong> Initial assessment and planning</li><li><strong>July-September:</strong> First advance tax review</li><li><strong>October-December:</strong> Mid-year review and adjustments</li><li><strong>January-March:</strong> Final tax-saving opportunities</li></ul>" }
        };

        int displayOrder = 0;
        foreach (var serviceData in services)
        {
            var createDto = new CreateServiceDto
            {
                Slug = serviceData.id,
                Name = serviceData.name,
                Type = serviceData.type,
                Level = serviceData.level,
                Description = serviceData.description,
                Highlight = serviceData.highlight,
                Content = serviceData.content,
                EnquiryTitle = "Interested in this service?",
                EnquirySubtitle = "Fill out the form below and our team will get back to you within one business day.",
                EnquiryButtonText = "Submit Enquiry",
                EnquiryNote = "On submission, we will send a confirmation email and our team will respond within one business day.",
                IsActive = true,
                DisplayOrder = displayOrder++
            };

            await _serviceService.CreateAsync(createDto);
        }

        _logger.LogInformation("Seeded {Count} services", services.Length);
    }

    private async Task SeedBlogPostsAsync(long adminUserId)
    {
        var existingBlogs = await _context.BlogPosts.CountAsync();
        if (existingBlogs > 0)
        {
            _logger.LogInformation("Blog posts already exist, skipping seed");
            return;
        }

        var blogPosts = new[]
        {
            new { id = "prepare-for-itr-season", title = "How to prepare for ITR season without last-minute stress", category = "Individuals", tags = new[] { "ITR", "Documentation", "Planning" }, readTime = "6 min read", excerpt = "A simple checklist to gather salary slips, AIS/26AS, and investment proofs so your return is filed in one shot.", content = "<p>Filing your Income Tax Return (ITR) doesn't have to be a last-minute scramble. With a bit of preparation and organization, you can make the process smooth and stress-free. Here's a practical checklist to help you gather everything you need well in advance.</p><h2>Essential Documents to Gather</h2><p>Start by collecting all your financial documents in one place. This includes:</p><ul><li><strong>Form 16:</strong> If you're a salaried employee, your employer will provide this. It contains details of your salary, TDS deducted, and other allowances.</li><li><strong>AIS (Annual Information Statement):</strong> Download this from the Income Tax portal. It shows all financial transactions reported by various entities.</li><li><strong>Form 26AS:</strong> This is your tax credit statement showing all TDS, TCS, advance tax, and self-assessment tax paid during the year.</li><li><strong>Bank Statements:</strong> Keep statements for all savings, current, and fixed deposit accounts.</li><li><strong>Investment Proofs:</strong> Documents for investments in ELSS, PPF, NSC, tax-saving FDs, life insurance premiums, health insurance, etc.</li></ul><h2>Additional Income Sources</h2><p>Don't forget to document any additional income you may have earned:</p><ul><li>Rental income receipts and property documents</li><li>Capital gains statements from brokers</li><li>Interest certificates from banks and post office</li><li>Dividend statements</li><li>Freelance or consulting income invoices</li></ul><h2>Deduction Documents</h2><p>To maximize your tax savings, ensure you have proof for all deductions:</p><ul><li>Section 80C investments (ELSS, PPF, NSC, etc.)</li><li>Section 80D health insurance premiums</li><li>Section 24(b) home loan interest certificates</li><li>Section 80G donation receipts</li><li>Section 80E education loan interest certificates</li></ul><h2>Pro Tips for Smooth Filing</h2><p>Create a dedicated folder (physical or digital) for all tax-related documents. Update it throughout the year as you receive documents. This way, when ITR season arrives, everything is already organized.</p><p>Review your Form 26AS and AIS early to identify any discrepancies. If you notice TDS not reflected, contact the deductor immediately to get it corrected.</p><p>If you're unsure about any aspect of your return, consult with a CA well before the deadline. This gives you time to gather any missing documents and file without rushing.</p>" },
            new { id = "new-tax-vs-old-regime", title = "New regime vs old regime: which one should you choose this year?", category = "Tax Planning", tags = new[] { "New Regime", "Old Regime" }, readTime = "5 min read", excerpt = "Understand how the slabs, exemptions, and deductions compare before you lock your choice for the year.", content = "<p>The choice between the new tax regime and the old tax regime is one of the most important decisions you'll make for your tax planning. Let's break down the key differences to help you make an informed choice.</p><h2>Understanding the New Tax Regime</h2><p>The new tax regime offers lower tax rates but comes with limited deductions. Key features include:</p><ul><li>Higher basic exemption limit of ₹3 lakh</li><li>Lower tax slabs across all income brackets</li><li>Most deductions under Chapter VI-A are not available (except Section 80CCD(2) and 80JJAA)</li><li>Standard deduction of ₹50,000 for salaried individuals</li><li>No deduction for HRA, LTA, or interest on home loans</li></ul><h2>Understanding the Old Tax Regime</h2><p>The old regime maintains higher tax rates but allows you to claim various deductions:</p><ul><li>Basic exemption limit of ₹2.5 lakh</li><li>Higher tax slabs</li><li>Full access to deductions under Section 80C, 80D, 24(b), 80G, etc.</li><li>HRA exemption available</li><li>Deduction for home loan interest under Section 24(b)</li><li>Standard deduction of ₹50,000</li></ul><h2>Who Should Choose What?</h2><p><strong>Choose the New Regime if:</strong></p><ul><li>You have minimal investments and deductions</li><li>Your total deductions are less than ₹1.5-2 lakh</li><li>You prefer simplicity and don't want to invest just for tax savings</li><li>You're a young professional with limited financial commitments</li></ul><p><strong>Choose the Old Regime if:</strong></p><ul><li>You have significant investments in tax-saving instruments</li><li>You have a home loan with substantial interest payments</li><li>You claim HRA and other exemptions</li><li>Your total deductions exceed ₹2-2.5 lakh</li><li>You're comfortable with more complex tax planning</li></ul><h2>Making the Decision</h2><p>The best way to decide is to calculate your tax liability under both regimes. Consider your actual investments, deductions, and exemptions. Remember, once you choose a regime for a financial year, you generally cannot switch back for that year.</p><p>If you're unsure, consult with a tax advisor who can help you run the numbers and make the optimal choice based on your specific financial situation.</p>" },
            new { id = "gst-small-business", title = "GST mistakes small businesses should avoid in their first year", category = "Business", tags = new[] { "GST", "Compliance" }, readTime = "7 min read", excerpt = "From incorrect registrations to missed reconciliations, we unpack frequent GST errors and how to prevent them.", content = "<p>Starting a business is exciting, but GST compliance can be overwhelming, especially in your first year. Here are the most common mistakes small businesses make and how to avoid them.</p><h2>1. Incorrect GST Registration</h2><p>Many businesses register under the wrong category or fail to register when required. Remember:</p><ul><li>If your annual turnover exceeds ₹20 lakh (₹10 lakh for special category states), GST registration is mandatory</li><li>Choose the correct business type (Regular, Composition, or E-commerce)</li><li>Ensure all business locations are properly registered</li><li>Update registration details immediately if business structure changes</li></ul><h2>2. Missing Filing Deadlines</h2><p>Late filing attracts penalties and interest. Key deadlines to remember:</p><ul><li>GSTR-1 (outward supplies): 11th of the following month</li><li>GSTR-3B (summary return): 20th of the following month</li><li>Annual return (GSTR-9): 31st December of the following financial year</li></ul><p>Set up calendar reminders and consider using GST software to automate reminders.</p><h2>3. Incorrect Invoice Format</h2><p>GST invoices must contain specific mandatory fields:</p><ul><li>GSTIN of supplier and recipient</li><li>Invoice number and date</li><li>HSN/SAC codes</li><li>Taxable value, CGST, SGST/IGST, and total amount</li><li>Place of supply</li></ul><p>Using incorrect formats can lead to mismatches and notices.</p><h2>4. Not Reconciling Books with Returns</h2><p>Regular reconciliation is crucial:</p><ul><li>Match your books of accounts with GSTR-1 and GSTR-3B</li><li>Reconcile input tax credit (ITC) with GSTR-2A/2B</li><li>Identify and correct discrepancies monthly</li><li>Maintain proper documentation for all transactions</li></ul><h2>5. Ignoring Input Tax Credit Rules</h2><p>Many businesses miss out on legitimate ITC or claim ineligible credits:</p><ul><li>Ensure invoices are uploaded by suppliers in GSTR-1</li><li>Verify that ITC is available in GSTR-2A/2B before claiming</li><li>Don't claim ITC on blocked items (personal expenses, etc.)</li><li>File returns on time to avoid ITC reversal</li></ul><h2>6. Not Maintaining Proper Records</h2><p>GST law requires maintaining detailed records:</p><ul><li>All invoices, credit notes, and debit notes</li><li>Purchase and sales registers</li><li>Stock records</li><li>Payment vouchers and receipts</li><li>All records must be maintained for at least 6 years</li></ul><h2>Best Practices</h2><p>Invest in good accounting software that integrates with GST. Consider hiring a CA or GST practitioner for the first year to ensure compliance. Regular training for your team on GST procedures is also essential.</p><p>Remember, prevention is better than cure. A small investment in proper compliance systems can save you from penalties, notices, and stress later.</p>" },
            new { id = "handle-tax-notice-calmly", title = "Received a tax notice? Here is how to respond calmly", category = "Support", tags = new[] { "Notices", "Income Tax" }, readTime = "4 min read", excerpt = "A step-by-step approach to understanding your notice, gathering documents, and drafting a clear response.", content = "<p>Receiving a tax notice can be stressful, but it's important to stay calm and respond systematically. Most notices are routine queries or requests for clarification. Here's how to handle them effectively.</p><h2>Step 1: Understand the Notice</h2><p>First, carefully read the notice to understand:</p><ul><li><strong>Notice Type:</strong> Is it a scrutiny notice, mismatch notice, or a simple query?</li><li><strong>Issue Raised:</strong> What specific point is the department questioning?</li><li><strong>Response Deadline:</strong> Note the date by which you must respond</li><li><strong>Required Documents:</strong> Check what documents or information is being requested</li></ul><p>Common notice types include Section 143(1) (intimation), Section 142(1) (scrutiny), Section 148 (reassessment), and mismatch notices under Section 139(9).</p><h2>Step 2: Gather Relevant Documents</h2><p>Once you understand what's being asked, gather all relevant documents:</p><ul><li>Copy of the original return filed</li><li>Supporting documents for the claim or transaction in question</li><li>Bank statements, invoices, receipts, or contracts as applicable</li><li>Any correspondence related to the matter</li><li>Form 26AS and AIS for verification</li></ul><h2>Step 3: Analyze the Issue</h2><p>Review your records to identify:</p><ul><li>Whether the notice is based on a genuine discrepancy</li><li>If it's a case of missing information or documentation</li><li>Whether there's an error in your return that needs correction</li><li>If the notice is incorrect and you need to contest it</li></ul><h2>Step 4: Draft Your Response</h2><p>Your response should be:</p><ul><li><strong>Clear and Concise:</strong> Address each point raised in the notice</li><li><strong>Well-Documented:</strong> Attach all relevant supporting documents</li><li><strong>Professional:</strong> Use proper language and format</li><li><strong>Timely:</strong> Submit before the deadline</li></ul><p>If the notice is incorrect, explain why with supporting evidence. If there's a genuine error, acknowledge it and provide corrected information.</p><h2>Step 5: Seek Professional Help</h2><p>For complex notices or if you're unsure how to respond:</p><ul><li>Consult with a Chartered Accountant</li><li>They can help interpret the notice correctly</li><li>Assist in drafting a proper response</li><li>Represent you before the tax authorities if needed</li></ul><h2>Important Reminders</h2><p>Never ignore a tax notice. Even if you think it's incorrect, you must respond within the specified time. Ignoring notices can lead to:</p><ul><li>Best judgment assessment</li><li>Penalties and interest</li><li>Legal complications</li></p><p>Keep copies of all correspondence and maintain a record of all interactions with the department. Respond through proper channels (online portal or registered post) and keep proof of submission.</p><p>Remember, most notices can be resolved with proper documentation and a clear explanation. Stay organized, respond promptly, and don't hesitate to seek professional assistance when needed.</p>" }
        };

        foreach (var blogData in blogPosts)
        {
            var createDto = new CreateBlogPostDto
            {
                Slug = blogData.id,
                Title = blogData.title,
                Category = blogData.category,
                Tags = blogData.tags.ToList(),
                ReadTime = blogData.readTime,
                Excerpt = blogData.excerpt,
                Content = blogData.content,
                IsPublished = true
            };

            await _blogService.CreateAsync(createDto, adminUserId);
        }

        _logger.LogInformation("Seeded {Count} blog posts", blogPosts.Length);
    }

    private async Task SeedFaqsAsync()
    {
        var existingFaqs = await _context.Faqs.CountAsync();
        if (existingFaqs > 0)
        {
            _logger.LogInformation("FAQs already exist, skipping seed");
            return;
        }

        var faqs = new[]
        {
            new { question = "Who should file an Income Tax Return through TaxHelperToday?", category = "ITR Filing", answer = "Any individual or business that has taxable income in India can file through TaxHelperToday. We work with salaried employees, freelancers, consultants, NRIs, and small businesses that prefer CA-led filing instead of doing it alone." },
            new { question = "What documents do I need to get started?", category = "ITR Filing", answer = "Typically you will need your PAN, Aadhaar, Form 16 (if salaried), bank statements, investment proofs, and details of any additional income such as rent, capital gains, or interest. Our team shares a checklist based on your profile." },
            new { question = "Which ITR form should I file?", category = "ITR Filing", answer = "The ITR form depends on your income sources and amount. ITR-1 is for salaried individuals with income up to ₹50 lakhs, ITR-2 for individuals with capital gains or multiple properties, ITR-3 for business income, and ITR-4 for presumptive taxation. Our CA will determine the correct form for you." },
            new { question = "What is the deadline for filing ITR?", category = "ITR Filing", answer = "For individuals, the deadline is typically July 31st of the assessment year. However, if you miss this deadline, you can file a belated return by December 31st with a penalty. We recommend filing before the deadline to avoid penalties and ensure faster refund processing." },
            new { question = "How long does it take to get a tax refund?", category = "ITR Filing", answer = "After e-verification, refunds are typically processed within 20-45 days by the Income Tax Department. If you have a valid bank account linked to your PAN, the refund is credited directly. We help track your refund status and ensure all documentation is in order." },
            new { question = "How does TaxHelperToday keep my data secure?", category = "Trust & Safety", answer = "We use secure, access-controlled workspaces, follow internal confidentiality protocols, and only request information that is required for compliance. You can choose preferred channels for sharing sensitive documents." },
            new { question = "Are the CAs qualified and registered?", category = "Trust & Safety", answer = "Yes, all Chartered Accountants on our platform are ICAI-registered and have valid practice certificates. We verify credentials before onboarding and maintain strict quality standards. You can request to see your CA's credentials if needed." },
            new { question = "Will my financial information be kept confidential?", category = "Trust & Safety", answer = "Absolutely. We follow strict confidentiality protocols as per ICAI guidelines. Your information is only shared with your assigned CA and support team on a need-to-know basis. We never share your data with third parties without your explicit consent." },
            new { question = "How long does it take to file my return?", category = "Process", answer = "Once we receive your complete information, most individual returns are prepared within 24–48 working hours. Timelines for business and complex returns are shared at onboarding." },
            new { question = "What is the step-by-step process for filing?", category = "Process", answer = "Step 1: Share your documents and answer our questionnaire. Step 2: Your assigned CA reviews and prepares your return. Step 3: You review the draft return and approve it. Step 4: We file it with the IT Department and provide you with the acknowledgment. Step 5: We help with e-verification." },
            new { question = "Can I revise my return if I made a mistake?", category = "Process", answer = "Yes, you can file a revised return within the assessment year or before the end of the relevant assessment year. We help identify errors and file revised returns when needed. There's no limit on the number of revisions, but it's best to get it right the first time." },
            new { question = "How do I e-verify my return?", category = "Process", answer = "E-verification can be done through Aadhaar OTP, Net Banking, or by generating an EVC through your bank. We guide you through the process and ensure your return is verified within 120 days of filing to be considered valid." },
            new { question = "Can you help if I have already received a notice?", category = "Support", answer = "Yes. We review the notice, retrieve relevant filings and data, and then help you draft a response and plan the next steps. Representation in front of authorities can also be arranged where required." },
            new { question = "What are your support hours?", category = "Support", answer = "Our support team is available Monday to Saturday, 9 AM to 7 PM IST. For urgent matters, you can reach out via email and we respond within 24 hours. Premium plan subscribers get priority support and extended hours." },
            new { question = "What is included in the pricing plans?", category = "Support", answer = "All plans include CA-led return preparation, accuracy guarantee, e-filing support, and basic notice assistance. Higher plans include priority support, year-round consultation, GST filing, and dedicated CA support. Check our pricing page for detailed comparisons." },
            new { question = "What payment methods do you accept?", category = "Support", answer = "We accept all major credit/debit cards, UPI, net banking, and digital wallets. Payment is required before we begin preparing your return. We also offer installment options for business plans." }
        };

        int displayOrder = 0;
        foreach (var faqData in faqs)
        {
            var createDto = new CreateFaqDto
            {
                Question = faqData.question,
                Answer = faqData.answer,
                Category = faqData.category,
                DisplayOrder = displayOrder++,
                IsActive = true
            };

            await _faqService.CreateAsync(createDto);
        }

        _logger.LogInformation("Seeded {Count} FAQs", faqs.Length);
    }

    private async Task SeedPagesAsync()
    {
        var existingPages = await _context.Pages.CountAsync();
        if (existingPages > 0)
        {
            _logger.LogInformation("Pages already exist, skipping seed");
            return;
        }

        // Full privacy policy content matching the HTML version
        var privacyPolicyContent = @"<div class=""section-heading"" style=""margin-bottom: 40px;"">
            <h2>Overview</h2>
            <p class=""section-subtitle"">
              At TaxHelperToday, we are committed to protecting your privacy and ensuring the security of your personal and financial information. This Privacy Policy explains how we collect, use, disclose, and safeguard your information when you use our website and services.
            </p>
            <p style=""margin-top: 20px;"">
              By using our services, you agree to the collection and use of information in accordance with this policy. We may update this Privacy Policy from time to time, and we will notify you of any changes by posting the new Privacy Policy on this page.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>1. Information We Collect</h2>
            <p>We collect information that is necessary to provide you with our tax and compliance services. The types of information we collect include:</p>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">1.1 Personal Information</h3>
            <ul class=""checklist"">
              <li>Name, date of birth, and gender</li>
              <li>PAN (Permanent Account Number) and Aadhaar number</li>
              <li>Contact information (email address, phone number, postal address)</li>
              <li>Bank account details and IFSC codes</li>
              <li>Employment and professional details</li>
            </ul>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">1.2 Financial Information</h3>
            <ul class=""checklist"">
              <li>Income details (salary, business income, capital gains, etc.)</li>
              <li>Investment and deduction details</li>
              <li>Tax payment records and TDS certificates</li>
              <li>Bank statements and financial documents</li>
              <li>Form 16, Form 26AS, and AIS (Annual Information Statement)</li>
            </ul>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">1.3 Technical Information</h3>
            <ul class=""checklist"">
              <li>IP address and browser type</li>
              <li>Device information and operating system</li>
              <li>Website usage data and cookies</li>
              <li>Log files and analytics data</li>
            </ul>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">1.4 Communication Records</h3>
            <ul class=""checklist"">
              <li>Correspondence via email, phone, or messaging</li>
              <li>Service requests and support interactions</li>
              <li>Feedback and survey responses</li>
            </ul>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>2. How We Use Your Information</h2>
            <p>We use the information we collect for the following purposes:</p>
            
            <div class=""two-column"" style=""margin-top: 32px;"">
              <div>
                <h3>📋 Service Delivery</h3>
                <p>To provide tax filing, compliance, and advisory services, including:</p>
                <ul class=""checklist"">
                  <li>Preparing and filing your income tax returns</li>
                  <li>Managing GST registration and returns</li>
                  <li>Handling TDS compliance and filings</li>
                  <li>Responding to tax notices and queries</li>
                </ul>
              </div>
              <div>
                <h3>🔒 Security & Compliance</h3>
                <p>To ensure security and meet legal obligations:</p>
                <ul class=""checklist"">
                  <li>Verifying your identity and preventing fraud</li>
                  <li>Complying with tax laws and regulations</li>
                  <li>Maintaining professional standards (ICAI)</li>
                  <li>Protecting against unauthorized access</li>
                </ul>
              </div>
              <div>
                <h3>💬 Communication</h3>
                <p>To keep you informed and provide support:</p>
                <ul class=""checklist"">
                  <li>Sending service updates and reminders</li>
                  <li>Responding to your inquiries</li>
                  <li>Providing customer support</li>
                  <li>Sharing important tax deadlines</li>
                </ul>
              </div>
              <div>
                <h3>📊 Service Improvement</h3>
                <p>To enhance our services and user experience:</p>
                <ul class=""checklist"">
                  <li>Analyzing usage patterns</li>
                  <li>Improving our website and processes</li>
                  <li>Developing new features</li>
                  <li>Conducting research and analytics</li>
                </ul>
              </div>
            </div>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>3. How We Share Your Information</h2>
            <p>We respect your privacy and do not sell your personal information. We may share your information only in the following circumstances:</p>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">3.1 With Your Consent</h3>
            <p>We may share your information when you explicitly authorize us to do so, such as when you request us to coordinate with other professionals or service providers.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">3.2 For Service Delivery</h3>
            <ul class=""checklist"">
              <li>With authorized team members and Chartered Accountants assigned to your case</li>
              <li>With service providers who assist us in delivering our services (under strict confidentiality agreements)</li>
              <li>With government authorities as required for tax filings and compliance</li>
            </ul>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">3.3 Legal Requirements</h3>
            <p>We may disclose your information if required by law, court order, or government regulation, or to protect our rights, property, or safety, or that of our clients.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">3.4 Business Transfers</h3>
            <p>In the event of a merger, acquisition, or sale of assets, your information may be transferred, but we will ensure the same level of privacy protection.</p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>4. Data Security</h2>
            <p style=""margin-bottom: 24px;"">
              We implement industry-standard security measures to protect your information from unauthorized access, alteration, disclosure, or destruction.
            </p>
            <div class=""two-column"" style=""margin-top: 32px;"">
              <div>
                <h3>🛡️ Technical Safeguards</h3>
                <ul class=""checklist"">
                  <li>SSL encryption for data transmission</li>
                  <li>Encrypted storage for sensitive documents</li>
                  <li>Secure access controls and authentication</li>
                  <li>Regular security assessments and updates</li>
                </ul>
              </div>
              <div>
                <h3>👥 Administrative Safeguards</h3>
                <ul class=""checklist"">
                  <li>Role-based access permissions</li>
                  <li>Confidentiality agreements for all staff</li>
                  <li>Regular access audits and reviews</li>
                  <li>Training on data protection practices</li>
                </ul>
              </div>
            </div>
            <p style=""margin-top: 24px; font-size: 14px; color: var(--color-text-muted);"">
              While we strive to protect your information, no method of transmission over the internet or electronic storage is 100% secure. We cannot guarantee absolute security but are committed to using commercially acceptable means to protect your data.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>5. Your Rights and Choices</h2>
            <p>You have the following rights regarding your personal information:</p>
            
            <div class=""two-column"" style=""margin-top: 32px;"">
              <div>
                <h3>Access & Correction</h3>
                <p>You can access, review, and update your personal information at any time. Contact us to request corrections to inaccurate or incomplete data.</p>
              </div>
              <div>
                <h3>Data Portability</h3>
                <p>You can request a copy of your data in a structured, machine-readable format for your records or to transfer to another service provider.</p>
              </div>
              <div>
                <h3>Deletion</h3>
                <p>You can request deletion of your personal information, subject to legal and professional obligations that may require us to retain certain records.</p>
              </div>
              <div>
                <h3>Opt-Out</h3>
                <p>You can opt out of marketing communications at any time by clicking unsubscribe links or contacting us directly. Service-related communications will continue.</p>
              </div>
            </div>
            
            <p style=""margin-top: 24px;"">
              To exercise any of these rights, please contact us at <a href=""mailto:mrhoque64@gmail.com"">mrhoque64@gmail.com</a> or call <a href=""tel:+918910397497"">+91-89103-97497</a>. We will respond to your request within 30 days.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>6. Data Retention</h2>
            <p>
              We retain your personal and financial information for as long as necessary to provide our services and comply with legal, regulatory, and professional obligations. This typically includes:
            </p>
            <ul class=""checklist"" style=""margin-top: 16px;"">
              <li>Tax returns and supporting documents: As required by tax laws (typically 6-7 years)</li>
              <li>Service records: For the duration of our engagement and as required by professional standards</li>
              <li>Communication records: For service delivery and dispute resolution purposes</li>
              <li>Legal requirements: As mandated by applicable laws and regulations</li>
            </ul>
            <p style=""margin-top: 20px;"">
              When information is no longer needed, we securely delete or anonymize it in accordance with our data retention policies.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>7. Cookies and Tracking Technologies</h2>
            <p>
              Our website uses cookies and similar tracking technologies to enhance your experience, analyze usage, and improve our services. Cookies are small text files stored on your device.
            </p>
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">Types of Cookies We Use</h3>
            <ul class=""checklist"">
              <li><strong>Essential Cookies:</strong> Required for the website to function properly</li>
              <li><strong>Analytics Cookies:</strong> Help us understand how visitors use our website</li>
              <li><strong>Preference Cookies:</strong> Remember your settings and preferences</li>
            </ul>
            <p style=""margin-top: 20px;"">
              You can control cookies through your browser settings. However, disabling certain cookies may affect website functionality.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>8. Third-Party Links and Services</h2>
            <p>
              Our website may contain links to third-party websites or services, such as government portals, payment gateways, or other service providers. We are not responsible for the privacy practices of these external sites. We encourage you to review their privacy policies before providing any information.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>9. Children's Privacy</h2>
            <p>
              Our services are not intended for individuals under the age of 18. We do not knowingly collect personal information from children. If you believe we have inadvertently collected information from a child, please contact us immediately so we can delete it.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>10. Changes to This Privacy Policy</h2>
            <p>
              We may update this Privacy Policy from time to time to reflect changes in our practices, technology, legal requirements, or other factors. We will notify you of significant changes by:
            </p>
            <ul class=""checklist"" style=""margin-top: 16px;"">
              <li>Posting the updated policy on this page with a new ""Last Updated"" date</li>
              <li>Sending an email notification to registered users</li>
              <li>Displaying a notice on our website</li>
            </ul>
            <p style=""margin-top: 20px;"">
              Your continued use of our services after such changes constitutes acceptance of the updated Privacy Policy.
            </p>
          </div>

          <div style=""margin-top: 48px; padding: 32px; background: var(--color-bg); border-radius: var(--radius-lg);"">
            <h2 style=""margin-top: 0;"">11. Contact Us</h2>
            <p>
              If you have any questions, concerns, or requests regarding this Privacy Policy or our data practices, please contact us:
            </p>
            <div style=""margin-top: 24px;"">
              <p><strong>TaxHelperToday</strong></p>
              <p>
                Email: <a href=""mailto:mrhoque64@gmail.com"">mrhoque64@gmail.com</a><br>
                Phone: <a href=""tel:+918910397497"">+91-89103-97497</a>
              </p>
            </div>
            <p style=""margin-top: 24px; font-size: 14px; color: var(--color-text-muted);"">
              We are committed to addressing your privacy concerns promptly and transparently.
            </p>
          </div>

          <div style=""margin-top: 32px; padding-top: 32px; border-top: 1px solid var(--color-border-subtle);"">
            <p style=""font-size: 14px; color: var(--color-text-muted); text-align: center;"">
              <strong>Last Updated:</strong> <span id=""privacy-last-updated"">January 2024</span>
            </p>
            <p style=""font-size: 14px; color: var(--color-text-muted); text-align: center; margin-top: 8px;"">
              For more information about our security practices, please visit our <a href=""/TrustSafety"">Trust & Safety</a> page.
            </p>
          </div>";

        // Privacy Policy with full content matching HTML version exactly
        var privacyPolicyDto = new CreatePageDto
        {
            Slug = "privacy-policy",
            Title = "Privacy Policy",
            Eyebrow = "Privacy Policy",
            HeroTitle = "How TaxHelperToday handles your information.",
            HeroText = "This page explains what data we collect, why we collect it, and how you can manage your preferences. Content on this page is fully manageable through the backend.",
            Content = privacyPolicyContent,
            LastUpdated = "January 2024",
            IsActive = true
        };
        await _pageService.CreateAsync(privacyPolicyDto);

        // Full Terms & Conditions content matching the HTML version
        var termsContent = @"<div class=""section-heading"" style=""margin-bottom: 40px;"">
            <h2>Overview</h2>
            <p class=""section-subtitle"">
              These Terms & Conditions (""Terms"") govern your use of TaxHelperToday's website and services. By accessing this website or engaging our services, you agree to be bound by these Terms. Please read them carefully before using our services.
            </p>
            <p style=""margin-top: 20px;"">
              TaxHelperToday is an independent tax and compliance advisory platform providing CA-led tax filing and compliance services. These Terms, along with our Privacy Policy and any service-specific engagement letters, constitute the complete agreement between you and TaxHelperToday.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>1. Acceptance of Terms</h2>
            <p>By accessing or using TaxHelperToday's website, services, or platform, you acknowledge that you have read, understood, and agree to be bound by these Terms and all applicable laws and regulations.</p>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">1.1 Agreement to Terms</h3>
            <p>If you do not agree with any part of these Terms, you must not use our services. Your use of our services constitutes acceptance of these Terms in their entirety.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">1.2 Modifications to Terms</h3>
            <p>We reserve the right to modify these Terms at any time. We will notify users of significant changes by posting the updated Terms on this page with a new ""Last Updated"" date. Your continued use of our services after such modifications constitutes acceptance of the updated Terms.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">1.3 Eligibility</h3>
            <p>You must be at least 18 years old and have the legal capacity to enter into contracts to use our services. By using our services, you represent and warrant that you meet these eligibility requirements.</p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>2. Description of Services</h2>
            <p>TaxHelperToday provides tax and compliance advisory services including, but not limited to:</p>
            
            <div class=""two-column"" style=""margin-top: 32px;"">
              <div>
                <h3>📋 Tax Filing Services</h3>
                <ul class=""checklist"">
                  <li>Income Tax Return (ITR) preparation and filing</li>
                  <li>Tax planning and optimization</li>
                  <li>Advance tax calculations</li>
                  <li>Tax refund assistance</li>
                </ul>
              </div>
              <div>
                <h3>🏢 Compliance Services</h3>
                <ul class=""checklist"">
                  <li>GST registration and return filing</li>
                  <li>TDS/TCS compliance and returns</li>
                  <li>Tax notice response and representation</li>
                  <li>Compliance calendar management</li>
                </ul>
              </div>
              <div>
                <h3>💼 Advisory Services</h3>
                <ul class=""checklist"">
                  <li>Year-round tax planning</li>
                  <li>Investment planning for tax savings</li>
                  <li>Tax consultation and guidance</li>
                  <li>Regime selection advice</li>
                </ul>
              </div>
              <div>
                <h3>📞 Support Services</h3>
                <ul class=""checklist"">
                  <li>Documentation assistance</li>
                  <li>E-verification support</li>
                  <li>Refund tracking</li>
                  <li>Query resolution</li>
                </ul>
              </div>
            </div>
            
            <p style=""margin-top: 24px;"">
              All services are provided by qualified Chartered Accountants (CAs) registered with the Institute of Chartered Accountants of India (ICAI). The specific services provided to you will be detailed in your service engagement letter or agreement.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>3. User Responsibilities and Obligations</h2>
            <p>As a user of TaxHelperToday's services, you have certain responsibilities:</p>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">3.1 Accurate Information</h3>
            <p>You are responsible for providing accurate, complete, and truthful information and documents. You must:</p>
            <ul class=""checklist"">
              <li>Disclose all sources of income and financial transactions</li>
              <li>Provide authentic and valid documents</li>
              <li>Update us promptly about any changes in your financial situation</li>
              <li>Ensure all information provided is current and correct</li>
            </ul>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">3.2 Document Submission</h3>
            <p>You must provide all required documents in a timely manner. Delays in document submission may affect service delivery timelines and deadlines.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">3.3 Review and Approval</h3>
            <p>You are responsible for reviewing all prepared returns, documents, and filings before they are submitted to tax authorities. Your approval indicates that you have reviewed and agree with the content.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">3.4 Compliance with Laws</h3>
            <p>You must comply with all applicable tax laws, regulations, and filing deadlines. While we assist you, ultimate responsibility for compliance rests with you.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">3.5 Account Security</h3>
            <p>If you have an account with us, you are responsible for maintaining the confidentiality of your account credentials and for all activities that occur under your account.</p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>4. Payment Terms</h2>
            <p>Our services are provided on a fee basis as per the pricing plans or service agreements:</p>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">4.1 Service Fees</h3>
            <ul class=""checklist"">
              <li>Fees are as per the pricing plan selected or as agreed in your service agreement</li>
              <li>All fees are in Indian Rupees (INR) unless otherwise specified</li>
              <li>Fees are subject to change, but changes will not affect ongoing engagements</li>
              <li>Additional services may incur additional charges as communicated</li>
            </ul>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">4.2 Payment Methods</h3>
            <p>We accept payment through credit/debit cards, UPI, net banking, and digital wallets. Payment is typically required before we begin preparing your return or providing services.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">4.3 Payment Obligations</h3>
            <p>You agree to pay all fees associated with the services you receive. Failure to pay may result in suspension or termination of services.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">4.4 Taxes</h3>
            <p>All fees are exclusive of applicable taxes (GST, etc.). You are responsible for any taxes applicable to the services provided.</p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>5. Refund and Cancellation Policy</h2>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">5.1 Service Cancellation</h3>
            <p>You may cancel your service request before we begin preparing your return. Cancellation requests must be made in writing via email or through our contact channels.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">5.2 Refund Eligibility</h3>
            <ul class=""checklist"">
              <li>Full refund if cancellation is made before we begin work on your return</li>
              <li>Partial refund may be available if cancellation is made after work has begun but before filing</li>
              <li>No refund after your return has been filed with the tax department</li>
              <li>Refunds, if applicable, will be processed within 7-14 business days</li>
            </ul>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">5.3 Non-Refundable Services</h3>
            <p>Once a return is filed, services are considered complete and no refunds will be provided. This includes cases where you may be dissatisfied with the outcome, as we provide services based on information you provide.</p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>6. Limitation of Liability and Disclaimers</h2>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">6.1 Service Limitations</h3>
            <p>While we follow robust review processes and employ qualified CAs, our services are based on the information and documents you provide. We cannot guarantee:</p>
            <ul class=""checklist"">
              <li>Elimination of all tax liability</li>
              <li>Prevention of all tax notices</li>
              <li>Specific refund amounts or timelines</li>
              <li>Outcomes of tax department assessments or audits</li>
            </ul>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">6.2 Limitation of Liability</h3>
            <p>To the maximum extent permitted by law, TaxHelperToday's liability is limited to the professional fees paid for the specific service in question. We shall not be liable for:</p>
            <ul class=""checklist"">
              <li>Indirect, incidental, or consequential damages</li>
              <li>Loss of profits, revenue, or business opportunities</li>
              <li>Penalties or interest imposed by tax authorities due to incorrect information provided by you</li>
              <li>Delays caused by factors beyond our control</li>
            </ul>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">6.3 Accuracy Disclaimer</h3>
            <p>While we strive for accuracy, tax laws are complex and subject to interpretation. Our services are provided based on our understanding of applicable laws at the time of service delivery. We cannot guarantee that tax authorities will accept all positions taken in your return.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">6.4 No Tax Advice Guarantee</h3>
            <p>Our services are advisory in nature. We provide assistance based on information you provide, but you remain responsible for the accuracy of your return and compliance with tax laws.</p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>7. Intellectual Property Rights</h2>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">7.1 Our Intellectual Property</h3>
            <p>All content on this website, including text, graphics, logos, software, and other materials, is the property of TaxHelperToday or its licensors and is protected by copyright, trademark, and other intellectual property laws.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">7.2 Limited License</h3>
            <p>You are granted a limited, non-exclusive, non-transferable license to access and use our website and services for personal or business tax compliance purposes only.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">7.3 Restrictions</h3>
            <p>You may not:</p>
            <ul class=""checklist"">
              <li>Copy, modify, or distribute our content without permission</li>
              <li>Use our content for commercial purposes other than your own tax compliance</li>
              <li>Reverse engineer or attempt to extract our proprietary information</li>
              <li>Remove copyright or proprietary notices from our content</li>
            </ul>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">7.4 Your Content</h3>
            <p>You retain ownership of all documents and information you provide to us. By using our services, you grant us a license to use your information solely for providing the services you have requested.</p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>8. Confidentiality and Privacy</h2>
            <p>We are committed to maintaining the confidentiality of your information:</p>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">8.1 Confidentiality Obligations</h3>
            <p>As a CA-led practice, we follow strict confidentiality protocols as per ICAI guidelines. Your financial and personal information is kept confidential and is only shared with authorized team members on a need-to-know basis.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">8.2 Privacy Policy</h3>
            <p>Our collection, use, and protection of your personal information is governed by our <a href=""/PrivacyPolicy"">Privacy Policy</a>, which forms part of these Terms.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">8.3 Data Security</h3>
            <p>We implement industry-standard security measures to protect your information. However, no method of transmission or storage is 100% secure, and we cannot guarantee absolute security.</p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>9. Service Modifications and Termination</h2>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">9.1 Service Modifications</h3>
            <p>We reserve the right to modify, suspend, or discontinue any part of our services at any time, with or without notice. We will make reasonable efforts to notify users of significant changes.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">9.2 Termination by You</h3>
            <p>You may discontinue using our services at any time. However, you remain responsible for any fees incurred for services already provided.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">9.3 Termination by Us</h3>
            <p>We may terminate or suspend your access to our services if:</p>
            <ul class=""checklist"">
              <li>You breach these Terms</li>
              <li>You fail to pay required fees</li>
              <li>You provide false or misleading information</li>
              <li>We are required to do so by law or regulatory authority</li>
            </ul>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>10. Dispute Resolution</h2>
            
            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">10.1 Good Faith Resolution</h3>
            <p>In case of any dispute, we encourage you to contact us first. We are committed to resolving disputes amicably through good faith discussions.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">10.2 Governing Law</h3>
            <p>These Terms are governed by the laws of India. Any disputes arising from these Terms or our services shall be subject to the exclusive jurisdiction of the courts in Kolkata, West Bengal, India.</p>

            <h3 style=""margin-top: 24px; margin-bottom: 16px;"">10.3 ICAI Guidelines</h3>
            <p>As a CA-led practice, we follow ICAI professional standards and ethical guidelines. Complaints regarding professional conduct can be raised with ICAI as per their procedures.</p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>11. Third-Party Services and Links</h2>
            <p>Our website may contain links to third-party websites or services, including:</p>
            <ul class=""checklist"">
              <li>Government tax portals (Income Tax Department, GST Portal)</li>
              <li>Payment gateways</li>
              <li>Other service providers</li>
            </ul>
            <p style=""margin-top: 16px;"">
              We are not responsible for the content, privacy practices, or terms of service of third-party websites. Your use of third-party services is subject to their respective terms and conditions.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>12. Force Majeure</h2>
            <p>
              We shall not be liable for any failure or delay in performance under these Terms due to circumstances beyond our reasonable control, including but not limited to natural disasters, government actions, internet failures, or other force majeure events.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>13. Severability</h2>
            <p>
              If any provision of these Terms is found to be invalid, illegal, or unenforceable, the remaining provisions shall continue in full force and effect. The invalid provision shall be replaced with a valid provision that most closely reflects the intent of the original provision.
            </p>
          </div>

          <div style=""margin-top: 48px;"">
            <h2>14. Entire Agreement</h2>
            <p>
              These Terms, together with our Privacy Policy and any service-specific engagement letters or agreements, constitute the entire agreement between you and TaxHelperToday regarding the use of our services and supersede all prior agreements and understandings.
            </p>
          </div>

          <div style=""margin-top: 48px; padding: 32px; background: var(--color-bg); border-radius: var(--radius-lg);"">
            <h2 style=""margin-top: 0;"">15. Contact Us</h2>
            <p>
              If you have any questions, concerns, or requests regarding these Terms & Conditions, please contact us:
            </p>
            <div style=""margin-top: 24px;"">
              <p><strong>TaxHelperToday</strong></p>
              <p>
                Email: <a href=""mailto:mrhoque64@gmail.com"">mrhoque64@gmail.com</a><br>
                Phone: <a href=""tel:+918910397497"">+91-89103-97497</a><br>
                Address: 1, Royd Ln, Esplanade, Taltala, Kolkata, West Bengal - 700016, India
              </p>
            </div>
            <p style=""margin-top: 24px; font-size: 14px; color: var(--color-text-muted);"">
              We are committed to addressing your concerns promptly and transparently.
            </p>
          </div>

          <div style=""margin-top: 32px; padding-top: 32px; border-top: 1px solid var(--color-border-subtle);"">
            <p style=""font-size: 14px; color: var(--color-text-muted); text-align: center;"">
              <strong>Last Updated:</strong> <span id=""terms-last-updated"">January 2024</span>
            </p>
            <p style=""font-size: 14px; color: var(--color-text-muted); text-align: center; margin-top: 8px;"">
              For information about how we handle your data, please review our <a href=""/PrivacyPolicy"">Privacy Policy</a>. For security information, visit our <a href=""/TrustSafety"">Trust & Safety</a> page.
            </p>
          </div>";

        var termsDto = new CreatePageDto
        {
            Slug = "terms-conditions",
            Title = "Terms & Conditions",
            Eyebrow = "Terms & Conditions",
            HeroTitle = "Understanding your relationship with TaxHelperToday.",
            HeroText = "These terms explain how our services work, the responsibilities on both sides, and important limitations. Content on this page is fully manageable through the backend.",
            Content = termsContent,
            LastUpdated = "January 2024",
                IsActive = true
            };
        await _pageService.CreateAsync(termsDto);

        // Full Trust & Safety content matching the HTML version
        var trustSafetyContent = @"<section class=""section section-alt"">
      <div class=""container"">
        <div class=""section-heading"">
          <h2>How we protect your data</h2>
          <p class=""section-subtitle"">
            We employ multiple layers of security to ensure your sensitive financial information remains protected throughout our engagement.
          </p>
        </div>
        <div class=""two-column"" style=""margin-top: 32px; grid-template-columns: repeat(2, 1fr);"">
          <div>
            <h3>🛡️ Document & Data Protection</h3>
            <p>All document exchanges use encrypted, secure pathways. Your information is stored in encrypted repositories with controlled access.</p>
            <ul class=""checklist"">
              <li>SSL-encrypted communication channels</li>
              <li>Encrypted file storage</li>
              <li>Secure document transfer methods</li>
            </ul>
          </div>
          <div>
            <h3>👤 Access Control</h3>
            <p>Only authorized team members assigned to your case can access your information. All access is on a need-to-know basis with regular reviews.</p>
            <ul class=""checklist"">
              <li>Role-based access permissions</li>
              <li>Confidentiality agreements required</li>
              <li>Regular access audits</li>
            </ul>
          </div>
          <div>
            <h3>⚖️ Professional Standards</h3>
            <p>As a CA-led practice, we adhere to ICAI professional standards and ethical guidelines for client information protection.</p>
            <ul class=""checklist"">
              <li>ICAI compliance</li>
              <li>Regular security assessments</li>
              <li>Industry-standard practices</li>
            </ul>
          </div>
          <div>
            <h3>✅ Quality Assurance</h3>
            <p>All filings undergo comprehensive verification and multi-stage validation to ensure accuracy and compliance.</p>
            <ul class=""checklist"">
              <li>Multi-stage quality checks</li>
              <li>Prompt status updates</li>
              <li>Thorough verification processes</li>
            </ul>
          </div>
        </div>
      </div>
    </section>

    <section class=""section"">
      <div class=""container"">
        <h2 style=""text-align: center; margin-bottom: 40px;"">Your information rights</h2>
        <p style=""text-align: center; max-width: 640px; margin: 0 auto 40px;"">
          You retain complete ownership and control of your information. We never share your data with third parties unless you explicitly authorize it. Your information is used solely for the tax and compliance services you've engaged us for.
        </p>
        <div class=""two-column"" style=""margin-top: 32px;"">
          <div>
            <h3>Privacy Guarantee</h3>
            <p>Your information is stored in secure, confidential systems. We do not distribute your data to external parties without your explicit written permission.</p>
            <ul class=""checklist"">
              <li>No third-party data sharing</li>
              <li>Permission-based access only</li>
              <li>Strict confidentiality protocols</li>
            </ul>
          </div>
          <div>
            <h3>Controlled Access</h3>
            <p>Only designated team members assigned to your case can access your information. Access is limited to what's necessary for your specific services.</p>
            <ul class=""checklist"">
              <li>Case-specific permissions</li>
              <li>Regular access reviews</li>
              <li>Secure information storage</li>
            </ul>
          </div>
        </div>
        <p style=""text-align: center; margin-top: 32px; font-size: 14px;"">
          For more details, see our <a href=""/PrivacyPolicy"">Privacy Policy</a> and <a href=""/TermsConditions"">Terms & Conditions</a>.
        </p>
      </div>
    </section>

    <section class=""section section-alt"">
      <div class=""container"">
        <h2 style=""text-align: center; margin-bottom: 40px;"">How you can stay safe</h2>
        <p style=""text-align: center; max-width: 640px; margin: 0 auto 40px;"">
          Your security is a shared responsibility. Follow these best practices to keep your information safe.
        </p>
        <div class=""two-column"" style=""margin-top: 32px;"">
          <div>
            <ul class=""checklist"">
              <li>Do not share OTPs or passwords with anyone, including our team</li>
              <li>Use dedicated email addresses and devices for financial accounts where possible</li>
              <li>Alert us immediately if you suspect unauthorised activity</li>
              <li>Update your contact details so we can reach you quickly for confirmations</li>
            </ul>
          </div>
          <div>
            <ul class=""checklist"">
              <li>Verify the identity of anyone claiming to be from TaxHelperToday</li>
              <li>Use strong, unique passwords for your accounts</li>
              <li>Enable two-factor authentication where available</li>
              <li>Review your account activity regularly</li>
            </ul>
          </div>
        </div>
      </div>
    </section>

    <section class=""section"">
      <div class=""container"">
        <div class=""section-heading"">
          <h2>Information Protection Questions</h2>
          <p class=""section-subtitle"">
            Frequently asked questions about our information protection measures and client safeguards.
          </p>
        </div>
        <div class=""accordion-list-modern"" style=""max-width: 800px; margin: 40px auto 0;"">
          <div class=""accordion-item"">
            <div class=""accordion-header"">
              <h3>What security measures protect my financial information?</h3>
              <span class=""accordion-toggle""><span class=""toggle-icon"">+</span></span>
            </div>
            <div class=""accordion-body"">
              <p>We use multi-layered security including SSL encryption, encrypted document transfers, role-based access controls, and regular security assessments. All communications are protected, and access is restricted to authorized staff only.</p>
            </div>
          </div>
          <div class=""accordion-item"">
            <div class=""accordion-header"">
              <h3>Who can access my information?</h3>
              <span class=""accordion-toggle""><span class=""toggle-icon"">+</span></span>
            </div>
            <div class=""accordion-body"">
              <p>Only authorized team members assigned to your case can access your data on a need-to-know basis. All team members sign confidentiality agreements and we regularly review access controls.</p>
            </div>
          </div>
          <div class=""accordion-item"">
            <div class=""accordion-header"">
              <h3>Do you share my information with third parties?</h3>
              <span class=""accordion-toggle""><span class=""toggle-icon"">+</span></span>
            </div>
            <div class=""accordion-body"">
              <p>No. We never share your data with third parties unless you explicitly authorize it. Your information is stored securely and used only for the services you've engaged us for.</p>
            </div>
          </div>
          <div class=""accordion-item"">
            <div class=""accordion-header"">
              <h3>What should I do if I suspect unauthorized activity?</h3>
              <span class=""accordion-toggle""><span class=""toggle-icon"">+</span></span>
            </div>
            <div class=""accordion-body"">
              <p>Contact us immediately at <a href=""tel:+918910397497"">+91-89103-97497</a> or <a href=""mailto:mrhoque64@gmail.com"">mrhoque64@gmail.com</a>. We investigate all security incidents promptly. We also recommend changing passwords and reviewing account activity.</p>
            </div>
          </div>
          <div class=""accordion-item"">
            <div class=""accordion-header"">
              <h3>What professional standards do you follow?</h3>
              <span class=""accordion-toggle""><span class=""toggle-icon"">+</span></span>
            </div>
            <div class=""accordion-body"">
              <p>As a CA-led practice, we follow ICAI professional standards and ethical guidelines. We implement industry-standard security practices, maintain secure communication channels, and conduct regular system assessments.</p>
            </div>
          </div>
        </div>
      </div>
    </section>";

        var trustSafetyDto = new CreatePageDto
        {
            Slug = "trust-safety",
            Title = "Trust & Safety",
            Eyebrow = "Trust & Safety",
            HeroTitle = "Your financial data deserves better protection.",
            HeroText = "TaxHelperToday is built on the belief that compliance and confidentiality go hand in hand. This page explains how we protect your data and how you can stay safe while working with us.",
            Content = trustSafetyContent,
            LastUpdated = "January 2024",
            IsActive = true
        };
        await _pageService.CreateAsync(trustSafetyDto);

        // About page - fully dynamic with structured data
        var statsJson = @"[
            {""icon"": ""👥"", ""value"": ""1M+"", ""label"": ""Trusted Users"", ""description"": ""Individuals & businesses across India""},
            {""icon"": ""🎓"", ""value"": ""3000+"", ""label"": ""CA Experts"", ""description"": ""Experienced professionals on our network""},
            {""icon"": ""✅"", ""value"": ""97.4%"", ""label"": ""Accuracy Rate"", ""description"": ""Get it right the first time""},
            {""icon"": ""⭐"", ""value"": ""4.9/5"", ""label"": ""Average Rating"", ""description"": ""From 6,800+ reviews""}
        ]";

        var ourStoryContent = @"<p>
            TaxHelperToday started as a small advisory practice focused on helping salaried
            individuals avoid last-minute tax rush. Over time, we saw the same challenges
            repeat across startups, family businesses, and professionals: scattered
            documentation, unclear communication, and surprise notices.
        </p>
        <p>
            We built TaxHelperToday to give taxpayers a clear, year-round view of their
            compliance position, while keeping the reassurance of a dedicated CA team.
        </p>";

        var howWeWorkChecklistJson = @"[
            ""Structured onboarding to understand your income, entities, and history"",
            ""Mapped compliance calendar for returns, registrations, and renewals"",
            ""Dedicated point-of-contact backed by a specialised CA panel"",
            ""Secure digital workspace for documents and approvals""
        ]";

        var missionCardsJson = @"[
            {""icon"": ""🎯"", ""title"": ""Empower Taxpayers"", ""description"": ""Give every Indian taxpayer access to expert CA guidance without the complexity and high costs of traditional firms.""},
            {""icon"": ""🔍"", ""title"": ""Ensure Accuracy"", ""description"": ""Every return is reviewed by qualified CAs to ensure 100% accuracy and compliance with Indian tax laws.""},
            {""icon"": ""🤝"", ""title"": ""Build Trust"", ""description"": ""Transparent processes, clear communication, and year-round support to build lasting relationships with our clients.""}
        ]";

        var whoWeServeContent = @"<p>
            Our team works with salaried individuals, first-time investors, freelancers,
            and growing businesses across India. Whether you only need help at filing time
            or ongoing advisory support, we tailor our engagement to your needs.
        </p>";

        var whoWeServeCategoriesJson = @"[
            {""icon"": ""💼"", ""label"": ""Salaried Individuals""},
            {""icon"": ""🚀"", ""label"": ""Startups""},
            {""icon"": ""👨‍💻"", ""label"": ""Freelancers""},
            {""icon"": ""🏢"", ""label"": ""Businesses""}
        ]";

        var valuesJson = @"[
            {""icon"": ""💡"", ""title"": ""Clarity"", ""subtitle"": ""over jargon""},
            {""icon"": ""🛡️"", ""title"": ""Compliance"", ""subtitle"": ""over shortcuts""},
            {""icon"": ""📅"", ""title"": ""Planning"", ""subtitle"": ""over panic""},
            {""icon"": ""🔒"", ""title"": ""Confidentiality"", ""subtitle"": ""over convenience""}
        ]";

        var teamMembersJson = @"[
            {""name"": ""Rajesh Kumar"", ""role"": ""Founder & Lead CA"", ""bio"": ""15+ years of experience in tax advisory and compliance. Specializes in corporate taxation and GST."", ""avatar"": ""👨‍💼""},
            {""name"": ""Priya Sharma"", ""role"": ""Senior CA - Individual Taxation"", ""bio"": ""Expert in personal income tax planning and ITR filing. Helps thousands of salaried professionals optimize their taxes."", ""avatar"": ""👩‍💼""},
            {""name"": ""Amit Patel"", ""role"": ""CA - Business Compliance"", ""bio"": ""Specializes in business registrations, GST compliance, and helping startups navigate tax regulations."", ""avatar"": ""👨‍💼""},
            {""name"": ""Sunita Reddy"", ""role"": ""CA - Audit & Assurance"", ""bio"": ""Ensures accuracy and compliance across all filings. Reviews every return before submission."", ""avatar"": ""👩‍💼""},
            {""name"": ""Vikram Singh"", ""role"": ""CA - International Taxation"", ""bio"": ""Expert in handling NRI taxation, DTAA, and cross-border tax matters for global professionals."", ""avatar"": ""👨‍💼""},
            {""name"": ""Anjali Mehta"", ""role"": ""Client Success Manager"", ""bio"": ""Your dedicated point of contact, ensuring seamless communication and timely support throughout your tax journey."", ""avatar"": ""👩‍💼""}
        ]";

        var aboutDto = new CreatePageDto
        {
            Slug = "about",
            Title = "About",
            Eyebrow = "About TaxHelperToday",
            HeroTitle = "CA-driven tax support for modern India.",
            HeroText = "TaxHelperToday was created to bridge the gap between traditional CA firms and do-it-yourself tax portals. We bring together structured processes, secure technology, and human expertise so that every filing is deliberate and defensible.",
            Content = "", // About page uses structured fields instead
            StatsJson = statsJson,
            OurStoryTitle = "Our story",
            OurStoryContent = ourStoryContent,
            HowWeWorkTitle = "How we work",
            HowWeWorkChecklistJson = howWeWorkChecklistJson,
            MissionEyebrow = "Our Mission",
            MissionTitle = "Making tax compliance simple, transparent, and stress-free",
            MissionCardsJson = missionCardsJson,
            WhoWeServeTitle = "Who we serve",
            WhoWeServeContent = whoWeServeContent,
            WhoWeServeCategoriesJson = whoWeServeCategoriesJson,
            WhatWeStandForTitle = "What we stand for",
            ValuesJson = valuesJson,
            TeamSectionTitle = "Our Team",
            TeamSectionSubtitle = "Our team of experienced CAs and tax professionals are dedicated to making your tax journey smooth and stress-free.",
            TeamMembersJson = teamMembersJson,
            LastUpdated = "January 2024",
            IsActive = true
        };
        await _pageService.CreateAsync(aboutDto);

        // Contact page - fully dynamic form sections
        var contactDto = new CreatePageDto
        {
            Slug = "contact",
            Title = "Contact Us",
            Eyebrow = "Contact Us",
            HeroTitle = "We're here to help with your tax questions.",
            HeroText = "Share your query and our team will get back to you with the next steps. For urgent compliance dates, please mention the relevant due date.",
            Content = "", // Empty - contact page has its own static layout
            ContactFormTitle = "Write to us",
            ContactFormDescription = "Fill out the form below and we'll get back to you within one business day.",
            ContactFormButtonText = "Submit Enquiry",
            ContactFormNote = "On submission, we will send a confirmation email and our team will respond within one business day.",
            ContactReachUsTitle = "Reach us",
            ContactFindUsTitle = "Find us",
            ContactFindUsDescription = "Visit our office or get directions to our location",
            LastUpdated = "January 2024",
            IsActive = true
        };
        await _pageService.CreateAsync(contactDto);

        // Home page - fully dynamic with all sections
        var heroTrustBadgesJson = @"[
            {""icon"": ""⭐"", ""value"": ""4.9/5"", ""label"": ""From 6,800+ Reviews""},
            {""icon"": ""✓"", ""value"": ""100%"", ""label"": ""Accuracy Guarantee""},
            {""icon"": ""👥"", ""value"": ""1M+"", ""label"": ""Trusted Users""}
        ]";

        var heroMetricsJson = @"[
            {""metric"": ""clients"", ""value"": ""1200"", ""label"": ""Individuals & Businesses Guided""},
            {""metric"": ""savings"", ""value"": ""75"", ""label"": ""Tax Savings Identified""},
            {""metric"": ""years"", ""value"": ""10"", ""label"": ""Years of CA Experience""}
        ]";

        var statsBannerJson = @"{
            ""eyebrow"": ""Proven Track Record"",
            ""title"": ""Numbers that show how we help taxpayers succeed."",
            ""subtitle"": ""From first-time filers to growing businesses, TaxHelperToday combines expert guidance and robust processes to consistently deliver better outcomes."",
            ""stats"": [
                {""icon"": ""👥"", ""value"": ""1M+"", ""label"": ""Trusted Users"", ""description"": ""Individuals, freelancers & businesses across India""},
                {""icon"": ""💰"", ""value"": ""94%"", ""label"": ""Average Tax Savings"", ""description"": ""Clients who pay less tax versus their earlier filings""},
                {""icon"": ""⚡"", ""value"": ""24hr"", ""label"": ""Fast Filing"", ""description"": ""Most returns prepared & filed within one working day""},
                {""icon"": ""🎓"", ""value"": ""3000+"", ""label"": ""Tax Experts"", ""description"": ""Experienced CAs and tax professionals on our network""}
            ]
        }";

        var ctaBannerJson = @"{
            ""title"": ""File ITR in Just 3 Minutes"",
            ""text"": ""Get expert-assisted tax filing with 365 days support. Free notice compliance included."",
            ""buttonText"": ""Start Filing Now""
        }";

        var howItWorksJson = @"{
            ""eyebrow"": ""Simple Process"",
            ""title"": ""File your ITR in 4 simple steps"",
            ""subtitle"": ""Our streamlined process makes tax filing quick, easy, and stress-free."",
            ""steps"": [
                {""number"": 1, ""icon"": ""📝"", ""title"": ""Answer Simple Questions"", ""text"": ""Just a few minutes to share your tax details online. We'll handle the rest.""},
                {""number"": 2, ""icon"": ""👨‍💼"", ""title"": ""Meet Your CA Expert"", ""text"": ""Get matched with a qualified Chartered Accountant who reviews your return.""},
                {""number"": 3, ""icon"": ""✅"", ""title"": ""Review & Optimize"", ""text"": ""We maximize your deductions and ensure 100% accuracy before filing.""},
                {""number"": 4, ""icon"": ""📤"", ""title"": ""File & Get Refund"", ""text"": ""Your ITR is filed securely with the Income Tax Department. Track your refund.""}
            ]
        }";

        var whySectionJson = @"{
            ""eyebrow"": ""Why TaxHelperToday"",
            ""title"": ""Made for Indians who want peace of mind at tax time."",
            ""subtitle"": ""From salaried employees to high-growth startups, we combine CA-led oversight, structured checklists, and technology to get your filings right the first time."",
            ""features"": [
                {""icon"": ""👨‍💼"", ""title"": ""CA-Led, Not Bot-Led"", ""text"": ""Every return and compliance review is prepared or reviewed by an experienced Chartered Accountant who understands Indian tax law in depth.""},
                {""icon"": ""📅"", ""title"": ""Planned, Not Last-Minute"", ""text"": ""We help you plan investments and deductions across the year so that tax season is about filing, not firefighting.""},
                {""icon"": ""💰"", ""title"": ""Transparent, Flat Pricing"", ""text"": ""Clearly defined plans for individuals, professionals, and businesses. No surprise add-ons, no confusing fine print.""},
                {""icon"": ""📋"", ""title"": ""Notice-Ready Documentation"", ""text"": ""We maintain a clean documentation trail so that if a notice arrives, you already have the data and expert support to respond quickly.""}
            ]
        }";

        var pricingPlansJson = @"{
            ""eyebrow"": ""Transparent Pricing"",
            ""title"": ""Choose the plan that fits your needs"",
            ""subtitle"": ""All plans include expert CA review and 100% accuracy guarantee."",
            ""plans"": [
                {
                    ""name"": ""Essential"",
                    ""price"": ""999"",
                    ""period"": ""/year"",
                    ""description"": ""Perfect for salaried individuals"",
                    ""features"": [
                        ""ITR filing by CA expert"",
                        ""Tax optimization"",
                        ""Accuracy guarantee"",
                        ""Form 16 import"",
                        ""E-verification support""
                    ],
                    ""isFeatured"": false,
                    ""badge"": null
                },
                {
                    ""name"": ""Professional"",
                    ""price"": ""2,499"",
                    ""period"": ""/year"",
                    ""description"": ""Best for freelancers & professionals"",
                    ""features"": [
                        ""Everything in Essential"",
                        ""Multiple income sources"",
                        ""Capital gains handling"",
                        ""HRA optimization"",
                        ""Investment planning support"",
                        ""Priority CA support""
                    ],
                    ""isFeatured"": true,
                    ""badge"": ""Most Popular""
                },
                {
                    ""name"": ""Business"",
                    ""price"": ""4,999"",
                    ""period"": ""/year"",
                    ""description"": ""Complete solution for businesses"",
                    ""features"": [
                        ""Everything in Professional"",
                        ""GST filing support"",
                        ""TDS return filing"",
                        ""Business tax planning"",
                        ""Dedicated CA support"",
                        ""Year-round consultation""
                    ],
                    ""isFeatured"": false,
                    ""badge"": null
                }
            ]
        }";

        var trustSectionJson = @"{
            ""eyebrow"": ""Trust & Security"",
            ""title"": ""Your data, handled like your signature."",
            ""text"": ""TaxHelperToday follows strict confidentiality protocols, multi-factor verification for sensitive actions, and secure storage so that your financial data stays protected."",
            ""listItems"": [
                ""Bank-grade encryption for document uploads"",
                ""Access controlled team workspaces"",
                ""Audit-ready logs of filings and approvals""
            ],
            ""badge"": {
                ""label"": ""Designed for"",
                ""title"": ""Zero Surprise Notices""
            },
            ""badgeText"": ""Clean records, timely filings, and proactive corrections reduce the chances of avoidable tax notices and stressful follow-ups.""
        }";

        var testimonialsJson = @"{
            ""eyebrow"": ""Customer Reviews"",
            ""title"": ""Trusted by thousands of taxpayers"",
            ""subtitle"": ""See what our customers say about their experience with TaxHelperToday."",
            ""testimonials"": [
                {
                    ""rating"": ""⭐⭐⭐⭐⭐"",
                    ""text"": ""Excellent service! My CA was very professional and helped me save ₹45,000 in taxes. The entire process was smooth and stress-free."",
                    ""author"": {
                        ""name"": ""Rajesh Kumar"",
                        ""role"": ""Software Engineer, Bangalore""
                    }
                },
                {
                    ""rating"": ""⭐⭐⭐⭐⭐"",
                    ""text"": ""I've been using TaxHelperToday for 3 years now. They handle everything - from filing to responding to notices. Highly recommended!"",
                    ""author"": {
                        ""name"": ""Priya Sharma"",
                        ""role"": ""Freelancer, Mumbai""
                    }
                },
                {
                    ""rating"": ""⭐⭐⭐⭐⭐"",
                    ""text"": ""Fast turnaround, great support team, and my refund was processed quickly. Will definitely use their services again next year."",
                    ""author"": {
                        ""name"": ""Amrit Singh"",
                        ""role"": ""Business Owner, Delhi""
                    }
                },
                {
                    ""rating"": ""⭐⭐⭐⭐⭐"",
                    ""text"": ""As an NRI, tax filing was always complicated for me. TaxHelperToday made it so simple and their CA explained everything clearly. Worth every rupee!"",
                    ""author"": {
                        ""name"": ""Vikram Mehta"",
                        ""role"": ""NRI, Dubai""
                    }
                },
                {
                    ""rating"": ""⭐⭐⭐⭐⭐"",
                    ""text"": ""Received a tax notice and was worried. Their team helped me respond within 2 days and resolved it completely. Professional and reliable service!"",
                    ""author"": {
                        ""name"": ""Anjali Reddy"",
                        ""role"": ""Marketing Manager, Hyderabad""
                    }
                }
            ]
        }";

        var valuePropositionJson = @"{
            ""eyebrow"": ""What We Guarantee"",
            ""title"": ""What sets TaxHelperToday apart"",
            ""items"": [
                {
                    ""icon"": ""✅"",
                    ""title"": ""97.4% Get It Right First Time"",
                    ""text"": ""Expert-assisted ITR filing ensures accuracy on the first attempt""
                },
                {
                    ""icon"": ""🆓"",
                    ""title"": ""No Hidden Charges"",
                    ""text"": ""Fully transparent pricing with no surprise fees""
                },
                {
                    ""icon"": ""📞"",
                    ""title"": ""24/7 Expert Support"",
                    ""text"": ""Year-round query resolution and notice management""
                }
            ]
        }";

        var homeDto = new CreatePageDto
        {
            Slug = "home",
            Title = "Home",
            Eyebrow = "India's Trusted CA-Led Tax Companion",
            HeroTitle = "File taxes with confidence, not confusion.",
            HeroText = "TaxHelperToday blends Chartered Accountant expertise with smart tools to help you stay compliant, minimise tax outgo, and respond to notices without stress.",
            Content = "", // Home page uses structured JSON fields instead
            HeroTrustBadgesJson = heroTrustBadgesJson,
            HeroMetricsJson = heroMetricsJson,
            MiniEnquiryBadge = "Free Consultation",
            MiniEnquiryTitle = "Get a Free Compliance Health Check",
            MiniEnquiryDescription = "Share a few details and our team will review your tax position for missed deductions, compliance gaps, and upcoming deadlines.",
            MiniEnquiryButtonText = "Request Callback",
            MiniEnquiryNote = "No spam. A qualified expert will contact you within 24 hours.",
            StatsBannerJson = statsBannerJson,
            CtaBannerJson = ctaBannerJson,
            HowItWorksJson = howItWorksJson,
            WhySectionJson = whySectionJson,
            PricingPlansJson = pricingPlansJson,
            TrustSectionJson = trustSectionJson,
            TestimonialsJson = testimonialsJson,
            ValuePropositionJson = valuePropositionJson,
            LastUpdated = "February 2024",
            IsActive = true
        };
        await _pageService.CreateAsync(homeDto);

        // Blogs page - hero section
        var blogsDto = new CreatePageDto
        {
            Slug = "blogs",
            Title = "Blogs & Guides",
            Eyebrow = "Tax & Compliance Library",
            HeroTitle = "Practical guides for everyday tax questions.",
            HeroText = "Explore curated explainers, checklists, and updates written by our tax experts to help you make informed decisions.",
            Content = "",
            LastUpdated = "January 2024",
            IsActive = true
        };
        await _pageService.CreateAsync(blogsDto);

        // FAQs page - hero section and help card
        var faqsDto = new CreatePageDto
        {
            Slug = "faqs",
            Title = "FAQs",
            Eyebrow = "Frequently Asked Questions",
            HeroTitle = "Answers to common tax and compliance queries.",
            HeroText = "Browse topics below or use search to quickly find what you need. For questions specific to your case, please reach out to our team directly.",
            Content = "",
            FaqHelpCardTitle = "Still have questions?",
            FaqHelpCardDescription = "Can't find the answer you're looking for? Our team is here to help. Get in touch and we'll respond within one business day.",
            FaqHelpCardButtonText = "Contact Us",
            LastUpdated = "January 2024",
            IsActive = true
        };
        await _pageService.CreateAsync(faqsDto);

        _logger.LogInformation("Seeded 8 pages (Home, About, Privacy Policy, Terms & Conditions, Trust & Safety, Contact, Blogs, FAQs - all with full dynamic content)");
    }
}
