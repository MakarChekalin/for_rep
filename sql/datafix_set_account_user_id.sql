update accounts a
set user_id = m.user_id
from (
    select
        unnest(string_to_array(:'account_numbers', ',')) as account_number,
        unnest(string_to_array(:'user_ids', ',')) as user_id
) m
where a.number = m.account_number;
