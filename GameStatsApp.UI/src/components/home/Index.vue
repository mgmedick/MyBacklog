<template>
    <div>
        <div v-if="!linkedaccounts || linkedaccounts.length == 0">          
            <link-accounts :username="username"></link-accounts>
        </div>
        <div v-else>
            <span>Accounts are linked</span>
        </div>            
    </div>   
</template>
<script>
    export default {
        name: "Index",
        props: {
            username: String,
            linkedaccounts: Array
        },
        data: function () {
            return {
            };
        },
        computed: {},
        watch: {},
        created: function () {
        },
        methods: {},
    };
</script>






